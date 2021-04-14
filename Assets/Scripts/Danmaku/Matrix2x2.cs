using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Matrix2x2
{
    public float m00;
    public float m01;
    public float m10;
    public float m11;

    public static readonly Matrix2x2 zero = new Matrix2x2
    {
        m00 = 0,
        m01 = 0,
        m10 = 0,
        m11 = 0,
    };

    public static readonly Matrix2x2 identity = new Matrix2x2
    {
        m00 = 1,
        m01 = 0,
        m10 = 0,
        m11 = 1,
    };

    public Vector2 GetRow(int index)
    {
        switch (index)
        {
            case 0:
                return new Vector2(m00, m01);
            case 1:
                return new Vector2(m10, m11);
            default:
                throw new System.Exception("Index out of range.");
        }
    }

    public Vector2 GetColumn(int index)
    {
        switch (index)
        {
            case 0:
                return new Vector3(m00, m10);
            case 1:
                return new Vector3(m01, m11);
            default:
                throw new System.Exception("Index out of range.");
        }
    }

    public void SetRow(int index, Vector2 row)
    {
        switch (index)
        {
            case 0:
                m00 = row.x;
                m01 = row.y;
                return;
            case 1:
                m10 = row.x;
                m11 = row.y;
                return;
            default:
                throw new System.Exception("Index out of range.");
        }
    }

    public void SetColumn(int index, Vector2 column)
    {
        switch (index)
        {
            case 0:
                m00 = column.x;
                m10 = column.y;
                return;
            case 1:
                m01 = column.x;
                m11 = column.y;
                return;
            default:
                throw new System.Exception("Index out of range.");
        }
    }

    public static Matrix2x2 ScaleMatrix(float scale)
    {
        return new Matrix2x2
        {
            m00 = scale,
            m01 = 0,
            m10 = 0,
            m11 = scale
        };
    }

    public static Matrix2x2 ScaleMatrix(float xScale, float yScale)
    {
        return new Matrix2x2
        {
            m00 = xScale,
            m01 = 0,
            m10 = 0,
            m11 = yScale
        };
    }

    public static Matrix2x2 RotationMatrix(float radian)
    {
        float cos = Mathf.Cos(radian);
        float sin = Mathf.Sin(radian);
        return new Matrix2x2
        {
            m00 = cos,
            m01 = -sin,
            m10 = sin,
            m11 = cos
        };
    }

    public static Matrix2x2 OuterProduct(Vector2 a, Vector2 b)
    {
        return new Matrix2x2
        {
            m00 = a.x * b.x,
            m01 = a.x * b.y,
            m10 = a.y * b.x,
            m11 = a.y * b.y
        };
    }

    public Vector2 Transform(Vector2 vector)
    {
        return new Vector2
        {
            x = m00 * vector.x + m01 * vector.y,
            y = m10 * vector.x + m11 * vector.y
        };
    }

    public static Matrix2x2 operator +(Matrix2x2 m, Matrix2x2 n)
    {
        return new Matrix2x2
        {
            m00 = m.m00 + n.m00,
            m01 = m.m01 + n.m01,
            m10 = m.m10 + n.m10,
            m11 = m.m11 + n.m11
        };
    }

    public static Matrix2x2 operator -(Matrix2x2 m, Matrix2x2 n)
    {
        return new Matrix2x2
        {
            m00 = m.m00 - n.m00,
            m01 = m.m01 - n.m01,
            m10 = m.m10 - n.m10,
            m11 = m.m11 - n.m11
        };
    }

    public static Matrix2x2 operator *(Matrix2x2 m, Matrix2x2 n)
    {
        return new Matrix2x2
        {
            m00 = m.m00 * n.m00 + m.m01 * n.m10,
            m01 = m.m00 * n.m01 + m.m01 * n.m11,
            m10 = m.m10 * n.m00 + m.m11 * n.m10,
            m11 = m.m10 * n.m01 + m.m11 * n.m11
        };
    }

    public static Matrix2x2 operator *(float scalar, Matrix2x2 m)
    {
        return new Matrix2x2
        {
            m00 = m.m00 * scalar,
            m01 = m.m01 * scalar,
            m10 = m.m10 * scalar,
            m11 = m.m11 * scalar
        };
    }

    public static Matrix2x2 operator *(Matrix2x2 m, float scalar)
    {
        return new Matrix2x2
        {
            m00 = m.m00 * scalar,
            m01 = m.m01 * scalar,
            m10 = m.m10 * scalar,
            m11 = m.m11 * scalar
        };
    }

    public static Matrix2x2 operator /(float scalar, Matrix2x2 m)
    {
        return new Matrix2x2
        {
            m00 = m.m00 / scalar,
            m01 = m.m01 / scalar,
            m10 = m.m10 / scalar,
            m11 = m.m11 / scalar
        };
    }

    public static bool ValueEquals(Matrix2x2 m, Matrix2x2 n)
    {
        return (m.m00 == n.m00 && m.m01 == n.m01 && m.m10 == n.m10 && m.m11 == n.m11);
    }
}
