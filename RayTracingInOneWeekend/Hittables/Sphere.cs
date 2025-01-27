﻿namespace RayTracing.Hittables
{
    using System;
    using System.Numerics;
    using RayTracing.Materials;

    public record Sphere(Vector3 Center, float Radius, Material Material) : Hittable
    {
        public static (float U, float V) GetSphereUv(Vector3 point)
        {
            float theta = (float)Math.Acos(-point.Y);
            float phi = (float)Math.Atan2(-point.Z, point.X) + (float)Math.PI;
            return (phi / (2 * (float)Math.PI), theta / (float)Math.PI);
        }

        public override bool Hit(Ray ray, float tMin, float tMax, ref HitRecord hitRecord)
        {
            Vector3 originCenter = ray.Origin - this.Center;
            float a = ray.Direction.LengthSquared();
            float halfB = Vector3.Dot(originCenter, ray.Direction);
            float c = originCenter.LengthSquared() - (float)Math.Pow(this.Radius, 2);
            float discriminant = (float)Math.Pow(halfB, 2) - (a * c);
            if (discriminant < 0)
            {
                return false;
            }

            float squareroot = (float)Math.Sqrt(discriminant);
            float root = (float)(-halfB - squareroot) / a;
            if (root < tMin || tMax < root)
            {
                root = (float)(-halfB + squareroot) / a;
                if (root < tMin || tMax < root)
                {
                    return false;
                }
            }

            hitRecord.T = root;
            hitRecord.Point = ray.At(hitRecord.T);
            Vector3 outwardNormal = (hitRecord.Point - this.Center) / this.Radius;
            hitRecord.SetFaceNormal(ray, outwardNormal);
            (hitRecord.U, hitRecord.V) = Sphere.GetSphereUv(outwardNormal);
            hitRecord.Material = this.Material;
            return true;
        }

        public override  AxisAlignedBoundingBox? BoundingBox(float time0, float time1)
        {
            Vector3 radiusVec = new(this.Radius);
            return new AxisAlignedBoundingBox(this.Center - radiusVec, this.Center + radiusVec);
        }
    }
}
