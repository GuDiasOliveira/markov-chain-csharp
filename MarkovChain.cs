using System;
using System.Collections.Generic;

namespace MarkovChainCSharp
{
    public class MarkovChain<T>
    {
        private double[,] m_transitionMatrix;
        private List<T> m_states = new List<T>();

        public MarkovChain(double[,] transitionMatrix, List<T> states)
        {
            SetChain(transitionMatrix, states);
        }

        public MarkovChain(double[,] transitionMatrix, params T[] states)
        {
            SetChain(transitionMatrix, states);
        }

        public int Size
        {
            get
            {
                return m_transitionMatrix.GetLength(0);
            }
        }

        public double this[int i, int j]
        {
            get
            {
                return m_transitionMatrix[i, j];
            }
            set
            {
                m_transitionMatrix[i, j] = value;
            }
        }

        public double this[T fromState, T toState]
        {
            get
            {
                int i = m_states.IndexOf(fromState);
                int j = m_states.IndexOf(toState);
                return m_transitionMatrix[i, j];
            }
            set
            {
                int i = m_states.IndexOf(fromState);
                int j = m_states.IndexOf(toState);
                m_transitionMatrix[i, j] = value;
            }
        }

        public T this[int i]
        {
            get
            {
                return m_states[i];
            }
            set
            {
                if (m_states.Contains(value))
                    throw new ArgumentException("Cannot set duplicate states");
                m_states[i] = value;
            }
        }

        public List<T> States
        {
            get
            {
                List<T> list = new List<T>();
                list.AddRange(m_states);
                return list;
            }
            set
            {
                int size = Size;
                m_states.Clear();
                for (int i = 0; i < size; i++)
                {
                    T state = value[i];
                    if (m_states.Contains(state))
                        throw new ArgumentException("Cannot have duplicate states");
                    m_states.Add(state);
                }
            }
        }

        public double[,] Matrix
        {
            get
            {
                int size = Size;
                double[,] matrix = new double[size, size];
                for (int i = 0; i < size; i++)
                    for (int j = 0; j < size; j++)
                        matrix[i, j] = m_transitionMatrix[i, j];
                return matrix;
            }
            set
            {
                int size = Size;
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                        m_transitionMatrix[i, j] = value[i, j];
                }
            }
        }

        public void SetChain(double[,] transitionMatrix, IList<T> states)
        {
            int size = transitionMatrix.GetLength(0);
            m_transitionMatrix = new double[size, size];
            m_states.Clear();
            for (int i = 0; i < size; i++)
            {
                T state = states[i];
                if (m_states.Contains(state))
                    throw new ArgumentException("Cannot have duplicate states");
                m_states.Add(state);
                for (int j = 0; j < size; j++)
                    m_transitionMatrix[i,j] = transitionMatrix[i,j];
            }
        }

        public void SetChain(double[,] transitionMatrix, params T[] states)
        {
            List<T> list = new List<T>();
            list.AddRange(states);
            SetChain(transitionMatrix, list);
        }

        public void Normalize()
        {
            int size = Size;
            for (int i = 0; i < size; i++)
            {
                double rowSum = 0;
                for (int j = 0; j < size; j++)
                    rowSum += m_transitionMatrix[i, j];
                for (int j = 0; j < size; j++)
                    m_transitionMatrix[i, j] /= rowSum;
            }
        }

        public double[] GetPi(double[] p0, int timeInterval)
        {
            if (timeInterval < 0)
                return null;
            Normalize();
            int size = Size;
            double[] s = new double[size];
            for (int index = 0; index < size; index++)
                s[index] = p0[index];
            if (timeInterval == 0)
            {
                return s;
            }
            for (int time = 0; time < timeInterval; time++)
            {
                double[] sNext = new double[size];
                for (int index = 0; index < size; index++)
                {
                    sNext[index] = 0;
                    for (int k = 0; k < size; k++)
                    {
                        sNext[index] += s[k] * m_transitionMatrix[k, index];
                    }
                }
                for (int index = 0; index < size; index++)
                    s[index] = sNext[index];
            }
            return s;
        }

        public double GetProbability(T initialState, int timeInterval, Func<T, bool> probabilityConditionTester)
        {
            double p = 0;
            foreach (T state in m_states)
            {
                if (probabilityConditionTester(state))
                    p += GetProbability(initialState, state, timeInterval);
            }
            return p;
        }

        public double GetProbability(double[] p0, T state, int timeInterval)
        {
            int stateIndex = m_states.IndexOf(state);
            return GetPi(p0, timeInterval)[stateIndex];
        }

        public double GetProbability(T initialState, T state, int timeInterval)
        {
            int size = Size;
            int initialStateIndex = m_states.IndexOf(initialState);
            double[] initialStatesProbs = new double[size];
            initialStatesProbs[initialStateIndex] = 1;
            return GetProbability(initialStatesProbs, state, timeInterval);
        }
    }
}
