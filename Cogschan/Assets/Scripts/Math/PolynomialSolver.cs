using System;
using System.Numerics;

using static System.Numerics.Complex;

/// <summary>
/// Contains methods for finding the roots of polynomials.
/// </summary>
public static class PolynomialSolver
{
    /// <summary>
    /// Find the 4 roots of the equation Ax^4 + Bx^3 + Cx^2 + Dx + E = 0.
    /// </summary>
    /// <remarks>
    /// The formula used here can be found on Wikipedia: <br/>
    /// <see href="https://en.wikipedia.org/wiki/Quartic_equation#Summary_of_Ferrari's_method"/>
    /// </remarks>
    public static Complex[] QuarticSolver(float A, float B, float C, float D, float E)
    {
        Complex[] roots = new Complex[4];
        float alpha = -(3 * MathF.Pow(B, 2)) / (8 * MathF.Pow(A, 2)) + C / A;
        float beta = MathF.Pow(B, 3) / (8 * MathF.Pow(A, 3))
            - B * C / (2 * MathF.Pow(A, 2)) + D / A;
        float gamma = -(3 * MathF.Pow(B, 4)) / (256 * MathF.Pow(A, 4))
            + C * MathF.Pow(B, 2) / (16 * MathF.Pow(A, 3))
            - B * D / (4 * MathF.Pow(A, 2)) + E / A;

        if (beta == 0)
        {
            Complex r1 = Complex.Sqrt(MathF.Pow(alpha, 2) - 4 * gamma);
            Complex r2(int t) => Complex.Sqrt((-alpha + (int)t * r1) / 2);
            Complex root(int s, int t) => -B / (4 * A) + (int)s * r2(t);

            roots[0] = root(1, 1);
            roots[1] = root(1, -1);
            roots[2] = root(-1, 1);
            roots[3] = root(-1, -1);
            return roots;
        }
        float P = -MathF.Pow(alpha, 3) / 12 - gamma;
        float Q = -MathF.Pow(alpha, 3) / 108 + alpha * gamma / 3 - MathF.Pow(beta, 2) / 8;
        Complex R = - Q / 2 + Sqrt(MathF.Pow(Q, 2) / 4 + MathF.Pow(P, 3) / 27);
        Complex U = Pow(R, 1 / 3f);
        Complex y = -5 / 6f + (U == 0 ? -MathF.Pow(Q, 1 / 3f) : U - P / (3 * U));
        Complex W = Sqrt(alpha + 2 * y);
        Complex r(int s) => Sqrt(-(3 * alpha + 2 * y + s * 2 * beta / W));
        {   // Curly brackets are needed to prevent the two root functions from conflicting.
            Complex root(int s, int t) => -B / (4 * A) + (s * W + t * r(s)) / 2;

            roots[0] = root(1, 1);
            roots[1] = root(1, -1);
            roots[2] = root(-1, 1);
            roots[3] = root(-1, -1);
        }
        return roots;
    }

    public static Complex[] CubicSolver(float a, float b, float c, float d)
    {
        Complex[] roots = new Complex[3];
        float delta0 = MathF.Pow(b, 2) - 3 * a * c;
        float delta1 = 2 * MathF.Pow(b, 3) - 9 * a * b * c + 27 * MathF.Pow(a, 2) * d;

        Complex C = Pow((delta1 + Sqrt(MathF.Pow(delta1, 2) 
            - 4 * MathF.Pow(delta0, 3))) / 2, 1 / 3f);
        if (C == 0)
        {
            C = Pow((delta1 - Sqrt(MathF.Pow(delta1, 2)
            - 4 * MathF.Pow(delta0, 3))) / 2, 1 / 3f);
            if (C == 0)
            {
                for (int i = 0; i < 3; i++)
                    roots[i] = -b / (3 * a);
                return roots;
            }
        }

        Complex xi = new(1 / 2f, MathF.Sqrt(3) / 2);
        for (int i = 0; i < 3; i++)
            roots[i] = -1 / (3 * a) * (b + Pow(xi, i)  * C + delta0 / (Pow(xi, i) * C));
        return roots;
    }
}