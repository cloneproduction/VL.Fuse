﻿using System;
using System.Collections.Generic;
using Fuse;
using Fuse.ShaderFX;
using NUnit.Framework;
using Stride.Core.Mathematics;
using Stride.Graphics;

namespace PatchTests
{
    public static class TestGraph
    {
        [Test]
        public static void TestInputs()
        {
            
            var gpuValue0 = new GpuInput<float>();
            Console.WriteLine(gpuValue0.DeclarationList());
            var gpuValue1 = new GpuInput<float>();
            Console.WriteLine(gpuValue1.DeclarationList());

            var add = new OperatorNode<float, float>(new List<GpuValue<float>>
                {gpuValue0.Output, gpuValue1.Output, gpuValue0.Output},new ConstantValue<float>(0),"+");

            var gpuValue2 = new GpuInput<float>();
            var add2 = new OperatorNode<float,float>(new List<GpuValue<float>> {add.Output, gpuValue2.Output},new ConstantValue<float>(0),"+");

            var sin = new IntrinsicFunctionNode<float>(
                new List<AbstractGpuValue> {add.Output, gpuValue2.Output},
                "sin", new ConstantValue<float>(0));
            Console.WriteLine(sin.BuildSourceCode());
            Console.WriteLine(sin.InputList().Count);
            sin.InputList().ForEach(Console.WriteLine);
        }
        
        [Test]
        public static void TestDrawShaderGraph()
        {
            var gpuValue0 = new GpuInput<float>();
            var gpuValue1 = new GpuInput<float>();

            var add = new OperatorNode<float, float>(new List<GpuValue<float>> {gpuValue0.Output, gpuValue1.Output},new ConstantValue<float>(0),"+");

            var gpuValue2 = new GpuInput<float>();
            var add2 = new OperatorNode<float, float>(new List<GpuValue<float>> {add.Output, gpuValue2.Output},new ConstantValue<float>(0),"+");

            var sin = new IntrinsicFunctionNode<float>(
                new List<AbstractGpuValue> {add.Output, gpuValue2.Output},
                "sin", new ConstantValue<float>(0));
            var drawShader = new DrawShader(
                new Dictionary<string, AbstractGpuValue>
                {
                    {"ShadingPosition", sin.Output}
                }, 
                new Dictionary<string, AbstractGpuValue>
                {
                    {"ColorTarget", sin.Output}
                });
            Console.WriteLine(drawShader.ShaderCode);
        }

        [Test]
        public static void TestDeclarations()
        {
            var gpuValue0 = new GpuInput<float>();
            var gpuValue1 = new GpuInput<float>();

            var add = new OperatorNode<float, float>(new List<GpuValue<float>> {gpuValue0.Output, gpuValue1.Output},new ConstantValue<float>(0),"+");

            var gpuValue2 = new GpuInput<float>();
            var add2 = new OperatorNode<float, float>(new List<GpuValue<float>> {add.Output, gpuValue2.Output},new ConstantValue<float>(0),"+");
            Console.WriteLine(add2.DeclarationList());
        }

        [Test]
        public static void TestMixins()
        {
            var gpuValue0 = new GpuInput<float>();
            
            var customFunction = new MixinFunctionNode<Vector2>(
                new List<AbstractGpuValue>
                {
                    ConstantHelper.FromFloat<Vector2>(0.5f), 
                    gpuValue0.Output,
                    gpuValue0.Output
                },
                "voronoise2D", 
                ConstantHelper.FromFloat<Vector2>(0),
                "Voronoise"
            );
            Console.WriteLine(customFunction.MixinList());
            Console.WriteLine(customFunction.BuildSourceCode());
        }

        [Test]
        public static void TestOperatorNode()
        {
            var gpuValue0 = new GpuInput<float>();
            var gpuValue1 = new GpuInput<float>();
            var operatorNode = new OperatorNode<float, float>(new List<GpuValue<float>> {gpuValue0.Output, gpuValue1.Output},new ConstantValue<float>(0),"+");
            var operatorNodeWithNull = new OperatorNode<float, float>(new List<GpuValue<float>> {gpuValue0.Output, gpuValue1.Output, null},new ConstantValue<float>(0),"+");
            
            Console.WriteLine(operatorNode.BuildSourceCode());
            Console.WriteLine(operatorNodeWithNull.BuildSourceCode());
        }
        
        [Test]
        public static void TestHallo()
        {
            
            
            Console.WriteLine("HALLO");
        }

        [Test]
        public static void TestCustomFunctions()
        {
            
            var Base = new GpuInput<Vector3>();
            var blend = new GpuInput<Vector3>();
            var opacity = new GpuInput<Vector3>();
            
            var inputs = new List<AbstractGpuValue>
            {
                Base.Output,
                blend.Output,
                opacity.Output,
            };
            const string template = @"
        ${resultType} ${signature}(${resultType} Base, ${resultType} Blend, float Opacity)
        {
            ${resultType} result1 = 1.0 - 2.0 * (1.0 - Base) * (1.0 - Blend);
            ${resultType} result2 = 2.0 * Base * Blend;
            ${resultType} zeroOrOne = step(Blend, 0.5);
            ${resultType} Out = result2 * zeroOrOne + (1 - zeroOrOne) * result1;
            return lerp(Base, Out, Opacity);
        }
";
            
            var customFunction = new CustomFunctionNode<Vector4>(inputs, "blendModeHardLight",template, ConstantHelper.FromFloat<Vector4>(0));
            
            Console.WriteLine(customFunction.BuildSourceCode());
            Console.WriteLine(customFunction.FunctionMap().Count);
            Console.WriteLine(customFunction.Functions.Count);
            customFunction.FunctionMap().ForEach(kv => Console.WriteLine(kv.Value));
            Console.WriteLine(customFunction.FunctionMap());
        }

        [Test]
        public static void TestFloatJoins()
        {
            var gpuValue0 = new GpuInput<float>();
            var gpuValue1 = new GpuInput<float>();
            var gpuValue2 = new GpuInput<float>();
            var gpuValue3 = new GpuInput<float>();
            
            var join2 = new Float2Join(gpuValue0.Output, gpuValue1.Output);
            Console.WriteLine(join2.BuildSourceCode());
            var join2Null = new Float2Join(gpuValue0.Output, null);
            Console.WriteLine(join2Null.BuildSourceCode());
            
            var join3 = new Float3Join(gpuValue0.Output, gpuValue1.Output, gpuValue2.Output);
            Console.WriteLine(join3.BuildSourceCode());
            var join3Null = new Float3Join(gpuValue0.Output, gpuValue1.Output, null);
            Console.WriteLine(join3Null.BuildSourceCode());
            
            var join4 = new Float4Join(gpuValue0.Output, gpuValue1.Output, gpuValue2.Output, gpuValue3.Output);
            Console.WriteLine(join4.BuildSourceCode());
            var join4Null = new Float4Join(gpuValue0.Output, gpuValue1.Output, gpuValue2.Output, null);
            Console.WriteLine(join4Null.BuildSourceCode());
        }

        [Test]
        public static void TestMember()
        {
            var texCoord = new Semantic<Vector2>("TexCoord");
            var member = new GetMember<Vector2, float>(texCoord.Output, "x");
            Console.WriteLine(member.BuildSourceCode());
            
            var memberNull = new GetMember<Vector2, float>(null, "x");
            Console.WriteLine(memberNull.BuildSourceCode());
        }
        
        [Test]
        public static void TestTraversal()
        {
            var texCoord = new Semantic<Vector2>("TexCoord");
            var gpuValue0 = new GpuInput<Vector2>();
            var add = new OperatorNode<Vector2, Vector2>(texCoord.Output, gpuValue0.Output,ConstantHelper.FromFloat<Vector2>(0),"*");

            var memberX = new GetMember<Vector2, float>(add.Output,"x");
            var memberY = new GetMember<Vector2, float>(add.Output,"y");

            var float2Join = new Float2Join(memberX.Output, memberY.Output);
            
            var memberX1 = new GetMember<Vector2, float>(float2Join.Output,"x");
            var memberY1 = new GetMember<Vector2, float>(float2Join.Output,"y");
            
            
            var add1 = new OperatorNode<float, float>(memberX.Output, memberY1.Output,new ConstantValue<float>(0),"+");
            
            Console.WriteLine(add1.BuildSourceCode());
        }

        [Test]
        public static void TestAssign()
        {
            var gpuValue0 = new GpuInput<Vector2>();
            var gpuValue1 = new GpuInput<Vector2>();
            var add = new OperatorNode<Vector2, Vector2>(new List<GpuValue<Vector2>> {gpuValue0.Output, gpuValue1.Output},ConstantHelper.FromFloat<Vector2>(0),"+");
            var add2 = new OperatorNode<Vector2, Vector2>(new List<GpuValue<Vector2>> {gpuValue0.Output, gpuValue0.Output},ConstantHelper.FromFloat<Vector2>(0),"+");
            
            var assign = new AssignNode<Vector2>(add.Output, add2.Output);
            
            Console.WriteLine(assign.BuildSourceCode());
        }

        [Test]
        public static void Bits()
        {
            Console.WriteLine(((BufferFlags.UnorderedAccess & BufferFlags.UnorderedAccess) == BufferFlags.UnorderedAccess)+"");
        }

        [Test]
        public static void Main()
        {
            var gpuValue0 = new GpuInput<float>();
            Console.WriteLine(gpuValue0.DeclarationList());
            var gpuValue1 = new GpuInput<float>();
            Console.WriteLine(gpuValue1.DeclarationList());

            var add = new OperatorNode<float, float>(new List<GpuValue<float>>
                {gpuValue0.Output, gpuValue1.Output, gpuValue0.Output},new ConstantValue<float>(0),"+");

            var gpuValue2 = new GpuInput<float>();
            var add2 = new OperatorNode<float,float>(new List<GpuValue<float>> {add.Output, gpuValue2.Output},new ConstantValue<float>(0),"+");

            var sin = new IntrinsicFunctionNode<float>(
                new List<AbstractGpuValue> {add.Output, gpuValue2.Output},
                "sin", new ConstantValue<float>(0));
            Console.WriteLine(add2.BuildSourceCode());
            Console.WriteLine(sin.BuildSourceCode());
            Console.WriteLine(add2.DeclarationList());

            sin.InputList().ForEach(Console.WriteLine);

            var texCoord = new Semantic<Vector2>("TexCoord");
            var add3 = new OperatorNode<Vector2,Vector2>(new List<GpuValue<Vector2>> {texCoord.Output, texCoord.Output},ConstantHelper.FromFloat<Vector2>(0),"+");
            Console.WriteLine(add3.BuildSourceCode());

            var customFunction = new MixinFunctionNode<Vector2>(
                new List<AbstractGpuValue>
                {
                    ConstantHelper.FromFloat<Vector2>(0.5f), 
                    gpuValue0.Output,
                    gpuValue0.Output
                },
                "voronoise2D",
                ConstantHelper.FromFloat<Vector2>(0),
                "Voronoise"
            );
            Console.WriteLine(customFunction.MixinList());
            Console.WriteLine(customFunction.BuildSourceCode());
            
            
        }

        [Test]
        public static void TestToShaderFX()
        {
            var gpuValue0 = new GpuInput<float>();
            Console.WriteLine(gpuValue0.DeclarationList());
            var gpuValue1 = new GpuInput<float>();
            Console.WriteLine(gpuValue1.DeclarationList());

            var add = new OperatorNode<float, float>(new List<GpuValue<float>>
                {gpuValue0.Output, gpuValue1.Output, gpuValue0.Output},new ConstantValue<float>(0),"+");

            var gpuValue2 = new GpuInput<float>();
            var add2 = new OperatorNode<float,float>(new List<GpuValue<float>> {add.Output, gpuValue2.Output},new ConstantValue<float>(0),"+");

            var sin = new IntrinsicFunctionNode<float>(
                new List<AbstractGpuValue> {add.Output, gpuValue2.Output},
                "sin", new ConstantValue<float>(0));

            var toMaterial = new ToShaderFX<float>(gpuValue0.Output);
           Console.WriteLine(toMaterial.ShaderCode);
        }

        [Test]
        public static void TestCreation()
        {
            var _value = new GpuValue<Vector3>("bla");
            var getDeclareBaseType = typeof(DeclareValue<>);
            var dataType = new Type [] { _value.GetType().GetGenericArguments()[0]};
            var getDeclareType = getDeclareBaseType.MakeGenericType(dataType);
            var getDeclareInstance = Activator.CreateInstance(getDeclareType, new object[]{null} ) as AbstractShaderNode;
            Console.WriteLine(getDeclareInstance.AbstractOutput().GetType().Name);
        }
    }
}