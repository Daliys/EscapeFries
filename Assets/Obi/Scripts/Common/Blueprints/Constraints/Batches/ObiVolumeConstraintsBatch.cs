using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Obi
{
    [Serializable]
    public class ObiVolumeConstraintsBatch : ObiConstraintsBatch
    {
        protected IVolumeConstraintsBatchImpl m_BatchImpl; 

        /// <summary>
        /// index of the last triangle for each constraint.
        /// </summary>
        [HideInInspector] public ObiNativeIntList lastIndices = new ObiNativeIntList();           

        /// <summary>
        /// rest volume for each constraint.
        /// </summary>
        [HideInInspector] public ObiNativeFloatList restVolumes = new ObiNativeFloatList();            

        /// <summary>
        /// 2 floats per constraint: pressure and stiffness.
        /// </summary>
        [HideInInspector] public ObiNativeVector2List pressureStiffness = new ObiNativeVector2List();  

        public override Oni.ConstraintType constraintType
        {
            get { return Oni.ConstraintType.Volume; }
        }

        public override IConstraintsBatchImpl implementation
        {
            get { return m_BatchImpl; }
        }

        public ObiVolumeConstraintsBatch(ObiVolumeConstraintsData constraints = null) : base()
        {
        }

        public void AddConstraint(int[] triangles, float restVolume)
        {
            RegisterConstraint();

            particleIndices.AddRange(triangles);
            restVolumes.Add(restVolume);
            pressureStiffness.Add(new Vector2(1,0));
            lastIndices.Add((int)particleIndices.count / 3);
        }

        public override void Clear()
        {
            base.Clear();
            particleIndices.Clear();
            lastIndices.Clear();
            restVolumes.Clear();
            pressureStiffness.Clear();
        }

        public override void GetParticlesInvolved(int index, List<int> particles)
        {
            //TODO.
        }

        protected override void SwapConstraints(int sourceIndex, int destIndex)
        {
            lastIndices.Swap(sourceIndex, destIndex);
            restVolumes.Swap(sourceIndex, destIndex);
            pressureStiffness.Swap(sourceIndex, destIndex);
        }

        public override void Merge(ObiActor actor, IObiConstraintsBatch other)
        {
            var batch = other as ObiVolumeConstraintsBatch;
            var user = actor as IVolumeConstraintsUser;

            if (batch != null && user != null)
            {
                if (!user.volumeConstraintsEnabled)
                    return;

                int initialIndexCount = particleIndices.count;
                int numActiveTriangles = batch.activeConstraintCount > 0 ? batch.lastIndices[batch.activeConstraintCount-1] : 0;

                particleIndices.ResizeUninitialized(initialIndexCount + numActiveTriangles * 3);
                lastIndices.ResizeUninitialized(lastIndices.count + batch.activeConstraintCount);
                restVolumes.ResizeUninitialized(m_ActiveConstraintCount + batch.activeConstraintCount);
                pressureStiffness.ResizeUninitialized(m_ActiveConstraintCount + batch.activeConstraintCount);
                lambdas.ResizeInitialized(m_ActiveConstraintCount + batch.activeConstraintCount);

                restVolumes.CopyFrom(batch.restVolumes, 0, m_ActiveConstraintCount, batch.activeConstraintCount);
                pressureStiffness.CopyReplicate(new Vector2(user.pressure, user.compressionCompliance), m_ActiveConstraintCount, batch.activeConstraintCount);

                for (int i = 0; i < numActiveTriangles * 3; ++i)
                    particleIndices[initialIndexCount + i] = actor.solverIndices[batch.particleIndices[i]];

                for (int i = 0; i < batch.activeConstraintCount; ++i)
                    lastIndices[m_ActiveConstraintCount + i] = initialIndexCount/3 + batch.lastIndices[i];

                base.Merge(actor, other);
            }
        }

        public override void AddToSolver(ObiSolver solver)
        {
            // Create distance constraints batch directly.
            m_BatchImpl = solver.implementation.CreateConstraintsBatch(constraintType) as IVolumeConstraintsBatchImpl;

            if (m_BatchImpl != null)
                m_BatchImpl.SetVolumeConstraints(particleIndices, lastIndices, restVolumes, pressureStiffness, lambdas, m_ActiveConstraintCount);
        }

        public override void RemoveFromSolver(ObiSolver solver)
        {
            //Remove batch:
            solver.implementation.DestroyConstraintsBatch(m_BatchImpl as IConstraintsBatchImpl);
        }

        /*public override void AddToSolver(ObiSolver solver)
        {
            // create and add the implementation:
            if (m_Constraints != null && m_Constraints.implementation != null)
            {
                m_BatchImpl = m_Constraints.implementation.CreateConstraintsBatch();
            }

            if (m_BatchImpl != null)
            {
                lambdas.Clear();

				//for (int i = 0; i < particleIndices.count; i++)
					//particleIndices[i] = constraints.GetActor().solverIndices[m_Source.particleIndices[i]];

                for (int i = 0; i < restVolumes.count; i++)
					lambdas.Add(0);

				m_BatchImpl.SetVolumeConstraints(particleIndices, firstTriangle, restVolumes, pressureStiffness, lambdas, m_ConstraintCount);
                m_BatchImpl.SetActiveConstraints(m_ActiveConstraintCount);
            }
        }

        public override void RemoveFromSolver(ObiSolver solver)
        {
            if (m_Constraints != null && m_Constraints.implementation != null)
                m_Constraints.implementation.RemoveBatch(m_BatchImpl);
        }*/

        public void SetParameters(float compliance, float pressure)
        {
            Vector2 p = new Vector2(pressure, compliance);
            for (int i = 0; i < pressureStiffness.count; i++)
                pressureStiffness[i] = p;
        }
    }
}
