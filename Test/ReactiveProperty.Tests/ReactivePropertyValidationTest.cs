using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Codeplex.Reactive;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;

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
            var errors = new List<string>();
            target.RequiredProperty
                .ObserveErrorChanged
                .OfType<string>()
                .Subscribe(errors.Add);
            errors.Any().IsFalse();

            target.RequiredProperty.Value = "a";
            errors.Any().IsFalse();
            target.RequiredProperty.HasErrors.IsFalse();

            target.RequiredProperty.Value = null;
            errors.Count.Is(1);
            errors[0].Is("error!");
            target.RequiredProperty.HasErrors.IsTrue();
        }

        [TestMethod]
        public void BothTest()
        {
            var errors = new List<string>();
            target.BothProperty
                .ObserveErrorChanged
                .OfType<string>()
                .Subscribe(errors.Add);

            target.BothProperty.HasErrors.IsTrue();
            errors.Count.Is(0);

            target.BothProperty.Value = "a";
            target.BothProperty.HasErrors.IsFalse();
            errors.Count.Is(0);

            target.BothProperty.Value = "aaaaaa";
            target.BothProperty.HasErrors.IsTrue();
            errors.Count.Is(1);
        }

        [TestMethod]
        public void TaskTest()
        {
            var errors = new List<string>();
            target.TaskValidationTestProperty
                .ObserveErrorChanged
                .OfType<string>()
                .Subscribe(errors.Add);
            errors.Count.Is(0);

            target.TaskValidationTestProperty.Value = "a";
            target.TaskValidationTestProperty.HasErrors.IsFalse();
            errors.Count.Is(0);

            target.TaskValidationTestProperty.Value = null;
            target.TaskValidationTestProperty.HasErrors.IsTrue();
            errors.Count.Is(1);
        }

        [TestMethod]
        public async Task AsyncValidation_SuccessCase()
        {
            var source = new TaskCompletionSource<IEnumerable>();
            var p = new ReactiveProperty<string>()
                .SetValidateNotifyError(_ =>
                {
                    return source.Task;
                });

            var errors = new List<string>();
            p.ObserveErrorChanged
                .OfType<string>()
                .Subscribe(errors.Add);

            p.Value = "a"; // fire validation logic;
            p.HasErrors.IsFalse();
            errors.Count.Is(0);

            source.SetResult(null); // validation success!

            await Task.Run(() => 
            {
                p.HasErrors.IsFalse();
                errors.Count.Is(0);
            });
        }

        [TestMethod]
        public async Task AsyncValidation_FailedCase()
        {
            var source = new TaskCompletionSource<IEnumerable>();
            var p = new ReactiveProperty<string>()
                .SetValidateNotifyError(_ =>
                {
                    return source.Task;
                });

            var errors = new List<string>();
            p.ObserveErrorChanged
                .OfType<string>()
                .Subscribe(errors.Add);

            p.Value = "a"; // fire validation logic;
            p.HasErrors.IsFalse();
            errors.Count.Is(0);

            source.SetResult("error message"); // validation error!

            await Task.Run(() => 
            {
                // check state
                p.HasErrors.IsTrue();
                errors.Count.Is(1, "error count is 1");
                errors[0].Is("error message");
                p.GetErrors("Value").Is("error message");
            });
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
