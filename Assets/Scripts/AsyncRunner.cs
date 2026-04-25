using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AsyncRunner
{
    private static bool running;

    private static IEnumerator AsyncRoutine(IEnumerable coroutine)
    {
        while (running)
        {
            yield return null;
        }

        running = true;

        Stack<IEnumerator> stack = new Stack<IEnumerator>();
        stack.Push(coroutine.GetEnumerator());

        // Frame rate is 60fps, use 40% of a frame at most
        float timePerFrame = 0.4f / 60f;
        float yieldTime = Time.realtimeSinceStartup + timePerFrame;

        void ResetTimer()
        {
            yieldTime = Time.realtimeSinceStartup + timePerFrame;
        }

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

        running = false;
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