﻿using System;
using System.Threading.Tasks;

namespace Tranquire.Extensions
{
    /// <summary>
    /// Represents a question which result is transform by the given selector function
    /// </summary>
    /// <typeparam name="TSource">The result type of the source question</typeparam>
    /// <typeparam name="TResult">The result type of the selector function</typeparam>
    internal sealed class SelectManyQuestionAsyncToAction<TSource, TResult> : Tranquire.ActionBase<Task<TResult>>
    {
        private readonly SelectMany<IQuestion<Task<TSource>>, Task<TSource>, Task<IAction<TResult>>, Task<TResult>> _selectMany;

        /// <summary>Record Constructor</summary>
        /// <param name="question">The question to get the result from</param>
        /// <param name="selector">The function to apply of the question result.</param>
        public SelectManyQuestionAsyncToAction(IQuestion<Task<TSource>> question, Func<TSource, IAction<TResult>> selector)
        {
            if (question == null)
            {
                throw new ArgumentNullException(nameof(question));
            }
            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            _selectMany = SelectMany.Create(question, SelectMany.AsksFor<Task<TSource>>(), selector, SelectMany.ExecuteAsync<TResult>());
        }

        /// <inheritsdoc />
        public override string Name => _selectMany.Name;

        /// <inheritsdoc />
        protected override Task<TResult>ExecuteWhen(IActor actor) => _selectMany.Apply(actor);
    }
}
