using System;

/// <summary>
/// Contains methods for approximating the roots of functions.
/// </summary>
public static class RootFinder
{
    /// <summary>
    /// Approximates a root of the given function.
    /// </summary>
    /// <remarks>
    /// Info on Newton's Method can be found on Wikipedia: <br/>
    /// <see href="https://en.wikipedia.org/wiki/Newton%27s_method"/>
    /// </remarks>
    /// <param name="f"> The function we are finding the root of.</param>
    /// <param name="guess"> The initial guess of the root. </param>
    /// <param name="tolerance"> How close to the root we want to be. </param>
    /// <param name="epsilon"> The value used for calculating the derivative. Also the lower bound of the valid denominator. </param>
    public static Root NewtonsMethod(Func<float, float> f, float guess, float tolerance, float epsilon)
    {
        float fPrime(float x) => (f(x + epsilon) - f(x)) / epsilon;

        for (int i = 0; i < Constants.MAX_ITER; i++)
        {
            if (fPrime(guess) < epsilon)
                break;
            float next = guess - f(guess) / fPrime(guess);
            if (MathF.Abs(next - guess) < tolerance)
                return new(next, true);
            guess = next;
        }
        return new(guess, false);
    }

    /// <summary>
    /// Stores the roots value as well as its accuracy.
    /// </summary>
    public readonly struct Root
    {
        /// <summary>
        /// The value of the Root.
        /// </summary>
        public readonly float Value;
        /// <summary>
        /// Whether or not the root is within the requested tolerance.
        /// </summary>
        public readonly bool IsAccurate;

        /// <summary>
        /// The constructor for the <see cref="Root"/>.
        /// </summary>
        public Root(float value, bool isAccurate)
        {
            Value = value;
            IsAccurate = isAccurate;
        }

        public static implicit operator float(Root root) => root.Value;

        /// <summary>
        /// Calls the given function if the root isn't accurate
        /// </summary>
        /// <returns>
        /// The root this is being called on.
        /// </returns>
        public Root IfInaccurate(Action func)  
        { 
            if (!IsAccurate) { func(); }
            return this;
        }
    }
}