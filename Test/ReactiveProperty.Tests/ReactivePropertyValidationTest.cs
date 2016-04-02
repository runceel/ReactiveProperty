using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reactive.Bindings;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using Microsoft.Reactive.Testing;

namespace ReactiveProperty.Tests
{
    [TestClass]
    public class ReactivePropertyValidationTest
    {
        private TestTarget target;

        [TestInitialize]
        public void Initialize()
        {
            this.target = new TestTarget();
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.target = null;
        }

        [TestMethod]
        public void InitialState()
        {
            target.RequiredProperty.HasErrors.IsTrue();
        }

        [TestMethod]
        public void AnnotationTest()
        {
            var errors = new List<IEnumerable>();
            target.RequiredProperty
                .ObserveErrorChanged
                .Where(x => x != null)
                .Subscribe(errors.Add);
            errors.Count.Is(1);
            errors[0].Cast<string>().Is("error!");
            target.RequiredProperty.HasErrors.IsTrue();

            target.RequiredProperty.Value = "a";
            errors.Count.Is(1);
            target.RequiredProperty.HasErrors.IsFalse();

            target.RequiredProperty.Value = null;
            errors.Count.Is(2);
            errors[1].Cast<string>().Is("error!");
            target.RequiredProperty.HasErrors.IsTrue();
        }

        [TestMethod]
        public void BothTest()
        {
            IEnumerable error = null;
            target.BothProperty
                .ObserveErrorChanged
                .Subscribe(x => error = x);

            target.BothProperty.HasErrors.IsTrue();
            error.OfType<string>().Is("required");

            target.BothProperty.Value = "a";
            target.BothProperty.HasErrors.IsFalse();
            error.IsNull();

            target.BothProperty.Value = "aaaaaa";
            target.BothProperty.HasErrors.IsTrue();
            error.IsNotNull();
            error.Cast<string>().Is("5over");

            target.BothProperty.Value = null;
            target.BothProperty.HasErrors.IsTrue();
            error.Cast<string>().Is("required");
        }

        [TestMethod]
        public void TaskTest()
        {
            var errors = new List<IEnumerable>();
            target.TaskValidationTestProperty
                .ObserveErrorChanged
                .Where(x => x != null)
                .Subscribe(errors.Add);
            errors.Count.Is(1);
            errors[0].OfType<string>().Is("required");

            target.TaskValidationTestProperty.Value = "a";
            target.TaskValidationTestProperty.HasErrors.IsFalse();
            errors.Count.Is(1);

            target.TaskValidationTestProperty.Value = null;
            target.TaskValidationTestProperty.HasErrors.IsTrue();
            errors.Count.Is(2);
        }

        [TestMethod]
        public async Task AsyncValidation_SuccessCase()
        {
            var tcs     = new TaskCompletionSource<string>();
            var rprop   = new ReactiveProperty<string>().SetValidateNotifyError(_ => tcs.Task);

            IEnumerable error = null;
            rprop.ObserveErrorChanged.Subscribe(x => error = x);
            
            rprop.HasErrors.IsFalse();
            error.IsNull();

            rprop.Value = "dummy";  //--- push value
            tcs.SetResult(null);    //--- validation success!
            await Task.Yield();

            rprop.HasErrors.IsFalse();
            error.IsNull();
        }

        [TestMethod]
        public async Task AsyncValidation_FailedCase()
        {
            var tcs     = new TaskCompletionSource<string>();
            var rprop   = new ReactiveProperty<string>().SetValidateNotifyError(_ => tcs.Task);

            IEnumerable error = null;
            rprop.ObserveErrorChanged.Subscribe(x => error = x);
            
            rprop.HasErrors.IsFalse();
            error.IsNull();

            var errorMessage    = "error occured!!";
            rprop.Value         = "dummy";  //--- push value
            tcs.SetResult(errorMessage);    //--- validation error!
            await Task.Yield();

            rprop.HasErrors.IsTrue();
            error.IsNotNull();
            error.Cast<string>().Is(errorMessage);
            rprop.GetErrors("Value").Cast<string>().Is(errorMessage);
        }

        [TestMethod]
        public void AsyncValidation_ThrottleTest()
        {
            var scheduler   = new TestScheduler();
            var rprop       = new ReactiveProperty<string>()
                            .SetValidateNotifyError(xs =>
                            {
                                return  xs
                                        .Throttle(TimeSpan.FromSeconds(1), scheduler)
                                        .Select(x => string.IsNullOrEmpty(x) ? "required" : null);
                            });

            IEnumerable error = null;
            rprop.ObserveErrorChanged.Subscribe(x => error = x);

            scheduler.AdvanceTo(TimeSpan.FromMilliseconds(0).Ticks);
            rprop.Value = string.Empty;
            rprop.HasErrors.IsFalse();
            error.IsNull();

            scheduler.AdvanceTo(TimeSpan.FromMilliseconds(300).Ticks);
            rprop.Value = "a";
            rprop.HasErrors.IsFalse();
            error.IsNull();
            
            scheduler.AdvanceTo(TimeSpan.FromMilliseconds(700).Ticks);
            rprop.Value = "b";
            rprop.HasErrors.IsFalse();
            error.IsNull();
            
            scheduler.AdvanceTo(TimeSpan.FromMilliseconds(1100).Ticks);
            rprop.Value = string.Empty;
            rprop.HasErrors.IsFalse();
            error.IsNull();
            
            scheduler.AdvanceTo(TimeSpan.FromMilliseconds(2500).Ticks);
            rprop.HasErrors.IsTrue();
            error.IsNotNull();
            error.Cast<string>().Is("required");
        }

        [TestMethod]
        public void ErrorChangedNonePattern()
        {
            var errors = new List<IEnumerable>();
            var rprop = new ReactiveProperty<string>()
                .SetValidateNotifyError(x => string.IsNullOrWhiteSpace(x) ? "error" : null);
            // old version behavior
            rprop.ObserveErrorChanged.Skip(1).Subscribe(errors.Add);

            errors.Count.Is(0);

            rprop.Value = "OK";
            errors.Count.Is(1);
            errors.Last().IsNull();

            rprop.Value = null;
            errors.Count.Is(2);
            errors.Last().OfType<string>().Is("error");
        }
    }


    class TestTarget
    {
        [Required(ErrorMessage = "error!")]
        public ReactiveProperty<string> RequiredProperty { get; private set; }

        [StringLength(5, ErrorMessage = "5over")]
        public ReactiveProperty<string> BothProperty { get; private set; }

        public ReactiveProperty<string> TaskValidationTestProperty { get; private set; }

        public TestTarget()
        {
            this.RequiredProperty = new ReactiveProperty<string>()
                .SetValidateAttribute(() => RequiredProperty);

            this.BothProperty = new ReactiveProperty<string>()
                .SetValidateAttribute(() => BothProperty)
                .SetValidateNotifyError(s => string.IsNullOrWhiteSpace(s) ? "required" : null);

            this.TaskValidationTestProperty = new ReactiveProperty<string>()
                .SetValidateNotifyError(async s =>
                {
                    if (string.IsNullOrWhiteSpace(s))
                    {
                        return await Task.FromResult("required");
                    }
                    return await Task.FromResult((string) null);
                });
        }
    }
}
