using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AsyncRunner
{
    private static int runningCount;

    private static IEnumerator AsyncRoutine(IEnumerable coroutine)
    {
        runningCount++;

        Stack<IEnumerator> stack = new Stack<IEnumerator>();
        stack.Push(coroutine.GetEnumerator());

        // Frame rate is 60fps, use 40% of a frame at most
        float timePerFrame = 0.4f / 60f;
        float yieldTime;

        void ResetTimer()
        {
            yieldTime = Time.realtimeSinceStartup + timePerFrame / runningCount;
        }

        ResetTimer();

        while (stack.Count > 0)
        {
            IEnumerator enumerator = stack.Peek();

            if (!enumerator.MoveNext())
            {
                stack.Pop();
                continue;
            }

            object current = enumerator.Current;

            if (current is IEnumerable enumerable)
            {
                stack.Push(enumerable.GetEnumerator());
            }
            else if (current != null)
            {
                yield return current;
                ResetTimer();
            }
            else if (Time.realtimeSinceStartup > yieldTime)
            {
                yield return null;
                ResetTimer();
            }
        }

        runningCount--;
    }

    public static void RunAsync(IEnumerable coroutine)
    {
        GenerationManager.instance.StartCoroutine(AsyncRoutine(coroutine));
    }

    public static void RunSync(IEnumerable coroutine)
    {
        foreach (object item in coroutine)
        {
            if (item is IEnumerable enumerable)
            {
                RunSync(enumerable);
            }
        }
    }
}