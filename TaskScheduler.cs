using System;
using System.Collections.Generic;

namespace TaskSchedulerApp
{
    /// <summary>
    /// Manages tasks and their dependencies.
    /// </summary>
    public class TaskScheduler
    {
        private readonly Dictionary<string, TaskNode> tasks;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskScheduler"/> class.
        /// </summary>
        public TaskScheduler()
        {
            this.tasks = new Dictionary<string, TaskNode>();
        }

        /// <summary>
        /// Adds a task to the scheduler.
        /// </summary>
        /// <param name="taskName">The name of the task to add.</param>
        public void AddTask(string taskName)
        {
            if (string.IsNullOrWhiteSpace(taskName))
            {
                throw new ArgumentException("Task name cannot be null or whitespace.", nameof(taskName));
            }

            if (!this.tasks.ContainsKey(taskName))
            {
                this.tasks[taskName] = new TaskNode(taskName);
            }
        }

        /// <summary>
        /// Adds a dependency to a task.
        /// </summary>
        /// <param name="taskName">The name of the task.</param>
        /// <param name="dependencyName">The name of the dependency.</param>
        public void AddDependency(string taskName, string dependencyName)
        {
            if (string.IsNullOrWhiteSpace(taskName))
            {
                throw new ArgumentException("Task name cannot be null or whitespace.", nameof(taskName));
            }

            if (string.IsNullOrWhiteSpace(dependencyName))
            {
                throw new ArgumentException("Dependency name cannot be null or whitespace.", nameof(dependencyName));
            }

            if (!this.tasks.ContainsKey(taskName))
            {
                throw new ArgumentException($"Task '{taskName}' does not exist.");
            }

            if (!this.tasks.ContainsKey(dependencyName))
            {
                throw new ArgumentException($"Dependency '{dependencyName}' does not exist.");
            }

            this.tasks[taskName].AddDependency(this.tasks[dependencyName]);
        }

        /// <summary>
        /// Executes all tasks in the correct order based on dependencies.
        /// </summary>
        public void ExecuteTasks()
        {
            var executedTasks = new HashSet<string>();
            var executionOrder = new List<string>();
            var visited = new HashSet<string>();

            foreach (var task in this.tasks.Values)
            {
                if (!executedTasks.Contains(task.Name))
                {
                    ExecuteTask(task, executedTasks, executionOrder, visited);
                }
            }

            Console.WriteLine("Execution Order: " + string.Join(", ", executionOrder));
        }

        /// <summary>
        /// Recursively executes a task and its dependencies.
        /// </summary>
        /// <param name="task">The task to execute.</param>
        /// <param name="executedTasks">The set of tasks that have already been executed.</param>
        /// <param name="executionOrder">The list that records the order of task execution.</param>
        /// <param name="visited">The set of tasks currently being visited to detect cycles.</param>
        private void ExecuteTask(TaskNode task, HashSet<string> executedTasks, List<string> executionOrder, HashSet<string> visited)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (visited.Contains(task.Name))
            {
                throw new InvalidOperationException($"Circular dependency detected for task '{task.Name}'.");
            }

            if (task.IsExecuted)
            {
                return;
            }

            visited.Add(task.Name);

            foreach (var dependency in task.Dependencies)
            {
                if (!executedTasks.Contains(dependency.Name))
                {
                    ExecuteTask(dependency, executedTasks, executionOrder, visited);
                }
            }

            task.IsExecuted = true;
            executedTasks.Add(task.Name);
            executionOrder.Add(task.Name);

            visited.Remove(task.Name);
        }
    }
}