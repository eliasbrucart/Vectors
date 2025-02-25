﻿using System;
using UnityEngine;

namespace CustomMath
{
    public struct MyQuaternion : IEquatable<MyQuaternion>
    {
        public const float kEpsilon = 1E-06F;
        public float x;
        public float y;
        public float z;
        public float w;

        public static MyQuaternion identity { get { return new MyQuaternion(0, 0, 0, 1); } }
        //Retorna los valores en x, y & z en los que un objeto esta rotado en el mundo
        //Setea los valores de x, y & z en los que queremos rotar un objeto, para esto usamos nuestro metodo Euler y asi rotar correctamente
        public Vec3 eulerAngles 
        {
            get
            {
                Vec3 a = Vec3.Zero;
                a.y = Mathf.Atan2((2 * y * w) - (2 * x * z), 1 - (2 * (y * y)) - 2 * (z * z)) * Mathf.Rad2Deg;
                a.z = Mathf.Asin(2 * x * y + 2 * z * w) * Mathf.Rad2Deg;
                a.x = Mathf.Atan2(2 * x * w - 2 * y * z, 1 - 2 * (x * x) - 2 * (z * z)) * Mathf.Rad2Deg;
                return a;
            }
            set
            {
                MyQuaternion quat = MyQuaternion.Euler(value);
                x = quat.x;
                y = quat.y;
                z = quat.z;
                w = quat.w;
            }
        }
        //normalizamos el quaternion con la misma orientacion, pero ahora su magnitud pasa a ser 1
        public MyQuaternion normalized
        {
            get
            {
                return Normalize(this);
            }
        }

        //constructors
        public MyQuaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        //public MyQuaternion()
        //{
        //    this.x = 0;
        //    this.y = 0;
        //    this.z = 0;
        //    this.w = 0;
        //}

        public MyQuaternion(MyQuaternion quat)
        {
            this.x = quat.x;
            this.y = quat.y;
            this.z = quat.z;
            this.w = quat.w;
        }

        //retorna el valor de cada componente
        //setea el valor de cada componente
        public float this[int index] 
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    case 2:
                        return z;
                    case 3:
                        return w;
                    default:
                        throw new IndexOutOfRangeException("Fuera de rango! 0 -> 3");
                }
            } 
            set 
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Fuera de rango! 0 -> 3");
                }
            } 
        }

        //operators
        public static implicit operator Quaternion(MyQuaternion q)
        {
            return new Quaternion(q.x, q.y, q.z, q.w);
        }

        public static implicit operator MyQuaternion(Quaternion q)
        {
            return new MyQuaternion(q.x, q.y, q.z, q.w);
        }

        //methods
        #region Methods
        public static float Angle(MyQuaternion a, MyQuaternion b)
        {
            MyQuaternion inverse = MyQuaternion.Inverse(a);
            MyQuaternion result = b * inverse;
            float angle = Mathf.Acos(result.w) * 2.0f * Mathf.Rad2Deg;
            return angle;
        }

        //devuelve un quaternion que rota en determinado eje con un angulo que le pasemos
        public static MyQuaternion AngleAxis(float angle, Vec3 axis)
        {
            angle *= Mathf.Deg2Rad;
            axis.Normalize();
            MyQuaternion result = new MyQuaternion
            {
                x = axis.x * Mathf.Sin(angle * 0.5f),
                y = axis.y * Mathf.Sin(angle * 0.5f),
                z = axis.y * Mathf.Sin(angle * 0.5f),
                w = Mathf.Cos(angle * 0.5f)
            };
            result.Normalize();

            return result;
        }

        public static float Dot(MyQuaternion a, MyQuaternion b)
        {
            return (a.x * b.x) + (a.y * b.y) + (a.z * b.z) + (a.w * b.w);
        }
        //Retorna un quaternion que gira n grados al rededor de cada uno de los ejes
        //Calculamos el seno del angulo en cada una de las componentes
        //El valor de W sera el resultado de la multiplicacion de los cosenos de cada componenete restando
        //el valor de la multiplicacion de los senos de cada componenete
        //Esto se hace por que debemos pasar de Euler a Quaternion, no podemos rotar en Euler.
        public static MyQuaternion Euler(Vec3 euler)
        {
            float rad = Mathf.Deg2Rad;
            euler *= rad;
            MyQuaternion q = new MyQuaternion();
            q.x = Mathf.Sin(euler.x * 0.5f);
            q.y = Mathf.Sin(euler.y * 0.5f);
            q.z = Mathf.Sin(euler.z * 0.5f);
            q.w = Mathf.Cos(euler.x * 0.5f) * Mathf.Cos(euler.y * 0.5f) * Mathf.Cos(euler.z * 0.5f) - Mathf.Sin(euler.x * 0.5f) * Mathf.Sin(euler.y * 0.5f) * Mathf.Sin(euler.z * 0.5f);
            q.Normalize();
            return q;
        }

        public static MyQuaternion Euler(float x, float y, float z)
        {
            float rad = Mathf.Deg2Rad;
            x *= rad;
            y *= rad;
            z *= rad;
            MyQuaternion q = new MyQuaternion();
            q.x = Mathf.Sin(x * 0.5f);
            q.y = Mathf.Sin(y * 0.5f);
            q.z = Mathf.Sin(z * 0.5f);
            q.w = Mathf.Cos(x * 0.5f) * Mathf.Cos(y * 0.5f) * Mathf.Cos(z * 0.5f) - Mathf.Sin(x * 0.5f) * Mathf.Sin(y * 0.5f) * Mathf.Sin(z * 0.5f);
            q.Normalize();
            return q;
        }

        public static MyQuaternion EulerAngles(float x, float y, float z)
        {
            return Euler(new Vec3(x, y, z));
        }

        public static MyQuaternion EulerAngles(Vec3 euler)
        {
            return Euler(euler.x, euler.y, euler.z);
        }

        public static MyQuaternion FromToRotation(Vec3 from, Vec3 to)
        {
            Vec3 cross = Vec3.Cross(from, to);
            MyQuaternion result = MyQuaternion.identity;
            result.x = cross.x;
            result.y = cross.y;
            result.z = cross.z;
            result.w = Mathf.Sqrt(from.magnitude * to.magnitude) * Mathf.Sqrt(from.magnitude * to.magnitude) + Vec3.Dot(from, to);
            result.Normalize();
            return result;
        }

        public static MyQuaternion Inverse(MyQuaternion rotation)
        {
            MyQuaternion inverse = new MyQuaternion(-rotation.x, -rotation.y, -rotation.z, rotation.w);
            return inverse;
        }

        public static MyQuaternion Lerp(MyQuaternion a, MyQuaternion b, float t)
        {
            MyQuaternion q = new MyQuaternion();
            if(t < 1)
            {
                q.x = ((b.x - a.x) * t + a.x);
                q.y = ((b.y - a.y) * t + a.y);
                q.z = ((b.z - a.z) * t + a.z);
                q.w = ((b.w - a.w) * t + a.w);
            }
            else
            {
                t = 1.0f;
            }
            q.Normalize();
            return q;
        }

        public static MyQuaternion LerpUnclamped(MyQuaternion a, MyQuaternion b, float t)
        {
            MyQuaternion q = new MyQuaternion();
            q.x = ((b.x - a.x) * t + a.x);
            q.y = ((b.y - a.y) * t + a.y);
            q.z = ((b.z - a.z) * t + a.z);
            q.w = ((b.w - a.w) * t + a.w);
            q.Normalize();
            return q;
        }

        public static MyQuaternion LookRotation(Vec3 forward)
        {
            return LookRotation(forward, Vec3.Up);
        }

        //crea una rotacion que orienta a un objeto que el forward de este mirando con la misma direccion que el forward de su objetivo
        public static MyQuaternion LookRotation(Vec3 forward, Vec3 upwards)
        {
            MyQuaternion result;
            if (forward == Vec3.Zero)
            {
                result = MyQuaternion.identity;
                return result;
            }
            if (upwards != forward)
            {
                upwards.Normalize();
                Vec3 a = forward + upwards * -Vec3.Dot(forward, upwards);
                MyQuaternion q = MyQuaternion.FromToRotation(Vec3.Forward, a);
                return MyQuaternion.FromToRotation(a, forward) * q;
            }
            else
            {
                return MyQuaternion.FromToRotation(Vec3.Forward, forward);
            }
        }

        public static MyQuaternion Normalize(MyQuaternion q)
        {
            float magnitude = Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
            q.x /= magnitude;
            q.y /= magnitude;
            q.z /= magnitude;
            q.w /= magnitude;
            return q;
        }

        public static MyQuaternion RotateTowards(MyQuaternion from, MyQuaternion to, float maxDegreesDelta)
        {
            throw new NotImplementedException();
        }

        public static MyQuaternion Slerp(MyQuaternion a, MyQuaternion b, float t)
        {
            MyQuaternion q = MyQuaternion.identity;
            a.Normalize();
            b.Normalize();
            float dot = Quaternion.Dot(a, b);
            if (dot < 0)
            {
                a = MyQuaternion.Inverse(a);
                dot = -dot;
            }
            float max = 0.9995f;
            if (dot > max)
            {
                MyQuaternion result = MyQuaternion.Lerp(a, b, t);
                result.Normalize();
                return result;
            }
            // si esta dentro del rango (0 a 1 0 0.99995)
            float angleT_0 = Mathf.Acos(dot);
            float angleT = angleT_0 * t;
            float sinT = Mathf.Sin(angleT);
            float sinT_0 = Mathf.Sin(angleT_0);

            float sin0 = Mathf.Cos(angleT) - dot * sinT / sinT_0;
            float sin1 = sinT / sinT_0;
            MyQuaternion res = MyQuaternion.identity;
            res.x = (a.x * sin0) + (b.x * sin1);
            res.y = (a.y * sin0) + (b.y * sin1);
            res.z = (a.z * sin0) + (b.z * sin1);
            res.w = (a.w * sin0) + (b.w * sin1);
            return res;
        }

        public static MyQuaternion SlerpUnclamped(MyQuaternion a, MyQuaternion b, float t)
        {
            MyQuaternion q = MyQuaternion.identity;
            a.Normalize();
            b.Normalize();
            float dot = Quaternion.Dot(a, b);

            float angleT_0 = Mathf.Acos(dot);
            float angleT = angleT_0 * t;
            float sinT = Mathf.Sin(angleT);
            float sinT_0 = Mathf.Sin(angleT_0);

            float sin0 = Mathf.Cos(angleT) - dot * sinT / sinT_0;
            float sin1 = sinT / sinT_0;
            MyQuaternion res = MyQuaternion.identity;
            res.x = (a.x * sin0) + (b.x * sin1);
            res.y = (a.y * sin0) + (b.y * sin1);
            res.z = (a.z * sin0) + (b.z * sin1);
            res.w = (a.w * sin0) + (b.w * sin1);
            return res;
        }

        public bool Equals(MyQuaternion other)
        {
            return x == other.x && y == other.y && z == other.z && w == other.w;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public void Normalize()
        {
            float magnitude = Mathf.Sqrt(x * x + y * y + z * z + w * w);
            x /= magnitude;
            y /= magnitude;
            z /= magnitude;
            w /= magnitude;
        }

        public void Set(float newX, float newY, float newZ, float newW)
        {
            x = newX;
            y = newY;
            z = newZ;
            w = newW;
        }

        public void SetFromToRotation(Vec3 fromDirection, Vec3 toDirection)
        {
            MyQuaternion q = FromToRotation(fromDirection, toDirection);
            x = q.x;
            y = q.y;
            z = q.z;
            w = q.w;
        }

        public void SetLookRotation(Vec3 view, Vec3 up)
        {
            MyQuaternion q = MyQuaternion.identity;
            q = LookRotation(view, up);
            x = q.x;
            y = q.y;
            z = q.z;
            w = q.w;
        }

        public void SetLookRotation(Vec3 view)
        {
            MyQuaternion quat = LookRotation(view);
            x = quat.x;
            y = quat.y;
            z = quat.z;
            w = quat.w;
        }

        public void ToAngleAxis(out float angle, out Vec3 axis)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "(" + x.ToString() + ", " + y.ToString() + ", " + z.ToString() + ", " + w.ToString() + ")";
        }

        #endregion

        #region operators

        public static Vec3 operator *(MyQuaternion rotation, Vec3 point)
        {
            MyQuaternion quatPoint = Euler(point);
            quatPoint *= rotation;
            return quatPoint.eulerAngles;
        }

        public static MyQuaternion operator *(MyQuaternion lhs, MyQuaternion rhs)
        {
            float x = (lhs.w * rhs.x) + (lhs.x * rhs.w) + (lhs.y * rhs.z) - (lhs.z * rhs.y);
            float y = (lhs.w * rhs.y) + (lhs.x * rhs.z) + (lhs.y * rhs.w) - (lhs.z * rhs.x);
            float z = (lhs.w * rhs.z) + (lhs.x * rhs.y) + (lhs.y * rhs.x) - (lhs.z * rhs.w);
            float w = (lhs.w * rhs.w) + (lhs.x * rhs.x) + (lhs.y * rhs.y) - (lhs.z * rhs.z);
            return new MyQuaternion(x,y,z,w);
        }

        public static bool operator ==(MyQuaternion lhs, MyQuaternion rhs)
        {
            return (lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z && lhs.w == rhs.w);
        }

        public static bool operator !=(MyQuaternion lhs, MyQuaternion rhs)
        {
            return (lhs.x != rhs.x || lhs.y != rhs.y || lhs.z != rhs.z || lhs.w != rhs.w);
        }

        #endregion
    }
}

