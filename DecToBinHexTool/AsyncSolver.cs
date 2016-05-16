﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace DecToBinHexTool
{
    class AsyncSolver : Solver
    {
        public delegate void UpdateResultDelegate(int idx, string result);
        public event UpdateResultDelegate UpdateResult;

        private class TaskDescription
        {
            public int Idx;
            public int Number;
            public int SecondsRemaining;
            public double TimeRemaining;
            public string Result;
        }

        private List<TaskDescription> _tasks;
        private BackgroundWorker _bw;

        public AsyncSolver()
        {
            _tasks = new List<TaskDescription>();
            _bw = new BackgroundWorker() {WorkerReportsProgress = true};

            _bw.DoWork += _bw_DoWork;
            _bw.ProgressChanged += _bw_ProgressChanged;
            _bw.RunWorkerCompleted += _bw_RunWorkerCompleted;
        }
        
        public void AddTask(int idx, int number, int secondsRemaining)
        {
            var task = new TaskDescription
            {
                Idx = idx,
                Number = number,
                SecondsRemaining = secondsRemaining,
                TimeRemaining = secondsRemaining,
                Result = string.Empty
            };

            var t = (ICollection)_tasks;
            lock (t.SyncRoot)
            {
                _tasks.Add(task);
            }

            if (!_bw.IsBusy)
            {
                _bw.RunWorkerAsync();
            }
        }

        private void _bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateResult?.Invoke(e.ProgressPercentage, (string)e.UserState);
        }

        private void _bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                throw new Exception(e.Error.Message);
            }
        }
        
        private void _bw_DoWork(object sender, DoWorkEventArgs e)
        {
            var currentTime = DateTime.Now;
            bool workComplete = false;

            while (!workComplete)
            {
                var elapsedTime = (DateTime.Now - currentTime).TotalSeconds;
                currentTime = DateTime.Now;

                var t = (ICollection)_tasks;
                lock (t.SyncRoot)
                {
                    for (var i = _tasks.Count - 1; i >= 0; i--)
                    {
                        var task = _tasks[i];
                        task.TimeRemaining -= elapsedTime;
                        
                        var secRemaining = (int)task.TimeRemaining + 1;
                        if (secRemaining < task.SecondsRemaining)
                        {
                            task.SecondsRemaining = secRemaining;
                            _bw.ReportProgress(task.Idx, Presenter.ProgressMessage(task.Number, secRemaining));
                        }

                        if (task.TimeRemaining <= 0)
                        {
                            if (task.Result == string.Empty)
                            {
                                task.Result = Compute(task.Number);
                            }
                            _bw.ReportProgress(task.Idx, Presenter.ResultMessage(task.Number, task.Result));
                            _tasks.RemoveAt(i);
                        }
                    }

                    workComplete = _tasks.Count == 0;
                }
                Thread.Sleep(100);
            }

        }
    }
}