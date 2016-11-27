﻿using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit2;
using System;
using Xunit;

namespace Tranquire.Tests
{
    public class ActionWithAbilityTests
    {
        public class TestAbility
        {
        }

        public class ActionExecuteWhen : Action<TestAbility, object>
        {
            public IActor Actor;
            private readonly IAction<object> _action;
            public TestAbility Ability;
            public ActionExecuteWhen(IAction<object> action)
            {
                _action = action;
            }

            protected override object ExecuteWhen(IActor actor, TestAbility ability)
            {
                Actor = actor;
                Ability = ability;
                return actor.Execute(_action);
            }

            protected override object ExecuteGiven(IActor actor, TestAbility ability)
            {
                return new object();
            }
        }

        public class ActionExecuteGiven : Action<TestAbility, object>
        {
            public IActor Actor;
            private readonly IAction<object> _action;
            public TestAbility Ability;
            public ActionExecuteGiven(IAction<object> action)
            {
                _action = action;
            }

            protected override object ExecuteWhen(IActor actor, TestAbility ability)
            {
                return new object();
            }

            protected override object ExecuteGiven(IActor actor, TestAbility ability)
            {
                Actor = actor;
                Ability = ability;
                return actor.Execute(_action);
            }
        }

        public class ActionExecuteWhenAndGivenNotOverridden : Action<TestAbility, object>
        {
            private readonly IAction<object> _action;
            public ActionExecuteWhenAndGivenNotOverridden(IAction<object> action)
            {
                _action = action;
            }

            protected override object ExecuteWhen(IActor actor, TestAbility ability)
            {
                return actor.Execute(_action);
            }
        }

        [Theory, DomainAutoData]
        public void Sut_ShouldBeAction(Action<TestAbility, object> sut)
        {
            Assert.IsAssignableFrom(typeof (IAction<TestAbility, TestAbility, object>), sut);
        }

        [Theory, DomainAutoData]
        public void Sut_VerifyGuardClauses(IFixture fixture)
        {
            var assertion = new GuardClauseAssertion(fixture);
            assertion.Verify(typeof (ActionExecuteGiven).GetMethods());
        }

        [Theory, DomainAutoData]
        public void ExecuteWhenAs_ShouldCallActorExecute(
            [Frozen] IAction<object> action,
            ActionExecuteWhen sut,
            Mock<IActor> actor,
            TestAbility ability,
            object expected)
        {
            //arrange
            actor.Setup(a => a.Execute(action)).Returns(expected);
            //act
            var actual = sut.ExecuteWhenAs(actor.Object, ability);
            //assert
            Assert.Equal(expected, actual);
        }

        [Theory, DomainAutoData]
        public void ExecuteGivenAs_ShouldCallActorExecute(
            [Frozen] IAction<object> action,
            ActionExecuteGiven sut,
            Mock<IActor> actor,
            TestAbility ability,
            object expected)
        {
            //arrange
            actor.Setup(a => a.Execute(action)).Returns(expected);
            //act
            var actual = sut.ExecuteGivenAs(actor.Object, ability);
            //assert
            Assert.Equal(expected, actual);
        }

        [Theory, DomainAutoData]
        public void ExecuteGivenAs_WhenExecuteGivenIsNotOverridden_ShouldCallActorExecute(
            [Frozen] IAction<object> action,
            ActionExecuteWhenAndGivenNotOverridden sut,
            Mock<IActor> actor,
            TestAbility ability,
            object expected)
        {
            //arrange
            actor.Setup(a => a.Execute(action)).Returns(expected);
            //act
            var actual = sut.ExecuteGivenAs(actor.Object, ability);
            //assert
            Assert.Equal(expected, actual);
        }

        [Theory, DomainAutoData]
        public void ExecuteWhenAs_ShouldUseCorrectActor(
            ActionExecuteWhen sut,
            Mock<IActor> actor,
            TestAbility ability)
        {
            //arrange
            var expected = actor.Object;
            //act
            sut.ExecuteWhenAs(actor.Object, ability);
            //assert
            Assert.Equal(expected, sut.Actor);
        }

        [Theory, DomainAutoData]
        public void ExecuteGivenAs_ShouldUseCorrectActor(ActionExecuteGiven sut, Mock<IActor> actor, TestAbility ability)
        {
            //arrange
            var expected = actor.Object;
            //act
            sut.ExecuteGivenAs(actor.Object, ability);
            //assert
            Assert.Equal(expected, sut.Actor);
        }

        [Theory, DomainAutoData]
        public void ExecuteWhenAs_ShouldUseCorrectAbility(ActionExecuteWhen sut, Mock<IActor> actor, TestAbility expected)
        {
            //arrange
            //act
            sut.ExecuteWhenAs(actor.Object, expected);
            //assert
            Assert.Equal(expected, sut.Ability);
        }

        [Theory, DomainAutoData]
        public void ExecuteGivenAs_ShouldUseCorrectAbility(ActionExecuteGiven sut, Mock<IActor> actor, TestAbility expected)
        {
            //arrange
            //act
            sut.ExecuteGivenAs(actor.Object, expected);
            //assert
            Assert.Equal(expected, sut.Ability);
        }
    }
}