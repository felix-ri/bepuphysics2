using System;
using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities;
using DemoContentLoader;
using DemoRenderer;

namespace Demos.Demos
{
    /// <summary>
    ///     A colosseum made out of boxes that is sometimes hit by large purple hail.
    /// </summary>
    public class OurDemo : Demo
    {
        public override void Initialize(ContentArchive content, Camera camera)
        {
            camera.Position = new Vector3(-30, 40, -30);
            camera.Yaw = MathHelper.Pi * 3f / 4;
            camera.Pitch = MathHelper.Pi * 0.2f;

            Simulation = Simulation.Create(BufferPool, new DemoNarrowPhaseCallbacks(),
                new DemoPoseIntegratorCallbacks(new Vector3(0, -10, 0)), new PositionFirstTimestepper());

            Simulation.Statics.Add(new StaticDescription(new Vector3(0, 0, 0),
                new CollidableDescription(Simulation.Shapes.Add(new Box(500, 1, 500)), 0.1f)));

            var ringBoxShape = new Box(1f, 1, 1);
            ringBoxShape.ComputeInertia(1, out var ringBoxInertia);
            var boxDescription = BodyDescription.CreateDynamic(new Vector3(), ringBoxInertia,
                new CollidableDescription(Simulation.Shapes.Add(ringBoxShape), 0.1f),
                new BodyActivityDescription(0.01f, 16));


            CreateCircleCube(25, 16, 11);
            CreateCircleCube(25, 14, 10);
            CreateCircleCube(25, 12, 10);
            CreateCircleCube(25, 10, 9);

            void CreateCircleCube(int sampleCount, float radius, int floorCount)
            {
                var wpCnt = sampleCount;
                var increment = (float)Math.PI * 2 / wpCnt;

                var offset = new Vector3(0, 0, 5);
                for (var j = 0; j < floorCount; j++)
                for (float i = 0; i < Math.PI * 2 - increment / 2; i += increment)
                {
                    var bd = boxDescription;
                    var pos = new Vector3((float)Math.Sin(i) * radius, j * 2, (float)Math.Cos(i) * radius);
                    bd.Pose.Position = pos + offset;
                    Simulation.Bodies.Add(bd);
                }
            }
        }
    }
}