using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.Constraints;
using BepuUtilities;
using BepuUtilities.Memory;
using DemoContentLoader;
using DemoRenderer;
using DemoRenderer.UI;
using DemoUtilities;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading;

namespace Demos.Demos
{
    /// <summary>
    /// CompoundStackDemo.
    /// </summary>
    public class CompoundStackDemo : Demo
    {
        public override void Initialize(ContentArchive content, Camera camera)
        {
            camera.Position = new Vector3(-2, 10, -10);
            camera.Yaw = MathHelper.Pi * 3f / 4;
            camera.Pitch = MathHelper.Pi * 0.2f;

            Simulation = Simulation.Create(BufferPool, new DemoNarrowPhaseCallbacks(new SpringSettings(60, 1)), new DemoPoseIntegratorCallbacks(new Vector3(0, -10, 0)), new SolveDescription(16, 2));


            TypedIndex compound;
            BodyInertia inertia;
            {
                using var compoundBuilder = new CompoundBuilder(BufferPool, Simulation.Shapes, 8);
                compoundBuilder.Add(new Box(3.15f,1.358f,0.03f),new Vector3(0,0,0.04f),1);
                compoundBuilder.Add(new Box(1.94f,1.35f,0.01f),new Vector3(0,0,0.008f),1);
                compoundBuilder.Add(new Box(0.34f,1.36f,0.04f),new Vector3(1.4f,0,0.018f),1);
                compoundBuilder.Add(new Box(0.34f,1.36f,0.04f),new Vector3(-1.4f,0,0.02f),1);
                compoundBuilder.Add(new Box(0.03f,0.89f,0.02f),new Vector3(-1.6f,-0.1f,0.05f),1);
                compoundBuilder.Add(new Box(0.03f,0.89f,0.02f),new Vector3(-1.6f,0.09f,0.03f),1);
                compoundBuilder.Add(new Box(0.03f,0.89f,0.02f),new Vector3(1.6f,-0.09f,0.03f),1);
                compoundBuilder.Add(new Box(0.04f,0.89f,0.02f),new Vector3(1.6f,-0.09f,0.05f),1);

                compoundBuilder.BuildDynamicCompound(out var children,out inertia, out var center);
                compound = Simulation.Shapes.Add(new Compound(children));
            }

            for (var i = 0; i < 10; i++) Simulation.Bodies.Add(BodyDescription.CreateDynamic(new RigidPose(new Vector3(0, .5f * i + .5f, 0), Quaternion.CreateFromYawPitchRoll(0, float.Pi / 2, 0)), inertia, compound, 0.01f));


            Simulation.Statics.Add(new StaticDescription(new Vector3(0, -0.5f, 0), Simulation.Shapes.Add(new Box(500, 1, 500))));

            var bulletShape = new Sphere(0.5f);
            bulletDescription = BodyDescription.CreateDynamic(new Vector3(), bulletShape.ComputeInertia(.1f), Simulation.Shapes.Add(bulletShape), 0.01f);

            var shootiePatootieShape = new Sphere(3f);
            shootiePatootieDescription = BodyDescription.CreateDynamic(new Vector3(), shootiePatootieShape.ComputeInertia(100), new(Simulation.Shapes.Add(shootiePatootieShape), 0.1f), 0.01f);
        }

        BodyDescription bulletDescription;
        BodyDescription shootiePatootieDescription;
        public override void Update(Window window, Camera camera, Input input, float dt)
        {
            if (input != null)
            {
                if (input.WasPushed(OpenTK.Input.Key.Z))
                {
                    bulletDescription.Pose.Position = camera.Position;
                    bulletDescription.Velocity.Linear = camera.GetRayDirection(input.MouseLocked, window.GetNormalizedMousePosition(input.MousePosition)) * 400;
                    Simulation.Bodies.Add(bulletDescription);
                }
                else if (input.WasPushed(OpenTK.Input.Key.X))
                {
                    shootiePatootieDescription.Pose.Position = camera.Position;
                    shootiePatootieDescription.Velocity.Linear = camera.GetRayDirection(input.MouseLocked, window.GetNormalizedMousePosition(input.MousePosition)) * 100;
                    Simulation.Bodies.Add(shootiePatootieDescription);
                }
            }
            base.Update(window, camera, input, dt);
        }

        public override void Render(Renderer renderer, Camera camera, Input input, TextBuilder text, Font font)
        {
            text.Clear().Append("Press Z to shoot a bullet, press X to super shootie patootie!");
            renderer.TextBatcher.Write(text, new Vector2(20, renderer.Surface.Resolution.Y - 20), 16, new Vector3(1, 1, 1), font);
            base.Render(renderer, camera, input, text, font);
        }
    }
}
