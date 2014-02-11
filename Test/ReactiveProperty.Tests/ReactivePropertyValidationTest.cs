using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Codeplex.Reactive;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Linq;
using System.Collections.Generic;

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
            target.BothProperty.HasErrors.IsTrue();

            target.BothProperty.Value = "a";
            target.BothProperty.HasErrors.IsFalse();

            target.BothProperty.Value = "aaaaaa";
            target.BothProperty.HasErrors.IsTrue();
        }

    }

    class TestTarget
    {
        [Required(ErrorMessage = "error!")]
        public ReactiveProperty<string> RequiredProperty { get; private set; }

        [StringLength(5, ErrorMessage = "5over")]
        public ReactiveProperty<string> BothProperty { get; private set; }

        public TestTarget()
        {
            this.RequiredProperty = new ReactiveProperty<string>()
                .SetValidateAttribute(() => RequiredProperty);

            this.BothProperty = new ReactiveProperty<string>()
                .SetValidateAttribute(() => BothProperty)
                .SetValidateNotifyError(s => string.IsNullOrWhiteSpace(s) ? "required" : null);
        }
    }
}
