using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Utility.WindowsForms.Forms;

namespace Utility.WindowsForms.CustomControls
{
    public class ProgressIndicator : IProgressIndicator
    {

        public static Action<ProgressIndicator> OnCreate;


        private static Task<ProgressIndicator> ThreadTask;
        private readonly List<ProgressIndicator> Subtasks = new List<ProgressIndicator>();
        public GroupBox gbSubTask;
        public Label lblStatus;
        public Panel mainProgressPanel;
        public FlowLayoutPanel panelMain;
        private Control parent;
        public ProgressBar pbProgress;
        private Action<Task<ProgressIndicator>> SubCreator;
        public Panel subTaskPanel;

        private ProgressIndicator SuperTask;

        private ProgressIndicator()
        {
        }

        public IProgressIndicator CreateSubTask(bool asTask = true)
        {
            if (!asTask)
            {
                return CreateProgressIndicator(subTaskPanel, SubCreator);
            }

            Task<ProgressIndicator> sTask =
                new Task<ProgressIndicator>(() => CreateProgressIndicator(subTaskPanel, SubCreator));
            SubCreator(sTask);
            while (!sTask.IsCompleted)
            {
                Thread.Sleep(100);
            }

            ProgressIndicator sub = sTask.Result;
            sub.panelMain.Location = new Point(
                                               sub.panelMain.Location.X,
                                               sub.panelMain.Location.Y + sub.panelMain.Size.Height * Subtasks.Count
                                              );
            sub.SuperTask = this;
            Subtasks.Add(sub);
            return sub;
        }

        public void SetProgress(string status, int currentProgress, int maxProgress)
        {
            lblStatus.Text = status;
            if (maxProgress != pbProgress.Maximum)
            {
                pbProgress.Maximum = maxProgress;
            }

            pbProgress.Value = currentProgress;

            Application.DoEvents();
        }

        public void Dispose()
        {
            SuperTask?.Subtasks.Remove(this);
            for (int i = Subtasks.Count - 1; i >= 0; i--)
            {
                Subtasks[i].Dispose();
            }

            panelMain.Parent = null;
            panelMain.Dispose();
        }


        public static ProgressIndicator CreateProgressIndicator(Control parent)
        {
            return CreateProgressIndicator(parent, SetThreadTask);
        }

        public static ProgressIndicator CreateProgressIndicator(
            Control parent, Action<Task<ProgressIndicator>> SubCreator)
        {
            ProgressIndicator i = new ProgressIndicator();
            i.SubCreator = SubCreator;
            i.parent = parent;
            i.panelMain = new FlowLayoutPanel();
            i.pbProgress = new ProgressBar();
            i.lblStatus = new Label();
            i.gbSubTask = new GroupBox();
            i.subTaskPanel = new Panel();
            i.mainProgressPanel = new Panel();
            i.panelMain.SuspendLayout();
            i.gbSubTask.SuspendLayout();
            i.mainProgressPanel.SuspendLayout();

            // 
            // panelMain
            // 
            i.panelMain.Controls.Add(i.mainProgressPanel);
            i.panelMain.Controls.Add(i.gbSubTask);
            i.panelMain.FlowDirection = FlowDirection.TopDown;
            i.panelMain.Location = new Point(12, 233);
            i.panelMain.Name = "panelMain";
            i.panelMain.Size = new Size(412, 257);
            i.panelMain.TabIndex = 0;
            i.panelMain.WrapContents = false;
            i.panelMain.Dock = DockStyle.Fill;
            i.parent.SizeChanged += (sender, args) => Parent_SizeChanged(i, args);

            // 
            // pbProgress
            // 
            i.pbProgress.Dock = DockStyle.Top;
            i.pbProgress.Location = new Point(0, 0);
            i.pbProgress.Margin = new Padding(0);
            i.pbProgress.Name = "pbProgress";
            i.pbProgress.Size = new Size(403, 23);
            i.pbProgress.Padding = new Padding(3, 3, 3, 3);
            i.pbProgress.TabIndex = 0;

            // 
            // lblStatus
            // 
            i.lblStatus.AutoSize = true;
            i.lblStatus.Location = new Point(3, 26);
            i.lblStatus.Margin = new Padding(3);
            i.lblStatus.Name = "lblStatus";
            i.lblStatus.Size = new Size(37, 13);
            i.lblStatus.TabIndex = 1;
            i.lblStatus.Text = "";

            // 
            // gbSubTask
            // 
            i.gbSubTask.Controls.Add(i.subTaskPanel);
            i.gbSubTask.Location = new Point(3, 55);
            i.gbSubTask.Name = "gbSubTask";
            i.gbSubTask.Size = new Size(406, 202);
            i.gbSubTask.TabIndex = 2;
            i.gbSubTask.TabStop = false;
            i.gbSubTask.Text = "Sub Task";

            // 
            // subTaskPanel
            // 
            i.subTaskPanel.Dock = DockStyle.Fill;
            i.subTaskPanel.Location = new Point(3, 16);
            i.subTaskPanel.Name = "subTaskPanel";
            i.subTaskPanel.Size = new Size(400, 183);
            i.subTaskPanel.TabIndex = 0;

            // 
            // mainProgressPanel
            // 
            i.mainProgressPanel.Controls.Add(i.pbProgress);
            i.mainProgressPanel.Controls.Add(i.lblStatus);
            i.mainProgressPanel.Dock = DockStyle.Top;
            i.mainProgressPanel.Location = new Point(3, 3);
            i.mainProgressPanel.Name = "mainProgressPanel";
            i.mainProgressPanel.Size = new Size(406, 46);
            i.mainProgressPanel.TabIndex = 2;
            i.mainProgressPanel.Margin = new Padding(3, 3, 3, 3);

            // 
            // temp
            // 
            i.panelMain.Parent = parent;

            i.panelMain.ResumeLayout(false);
            i.gbSubTask.ResumeLayout(false);
            i.mainProgressPanel.ResumeLayout(false);
            i.mainProgressPanel.PerformLayout();
            Parent_SizeChanged(i, EventArgs.Empty);
            OnCreate?.Invoke(i);
            return i;
        }

        public void UpdateBounds()
        {
            Parent_SizeChanged(this, EventArgs.Empty);
        }

        private static void Parent_SizeChanged(ProgressIndicator sender, EventArgs e)
        {
            sender.mainProgressPanel.Width = sender.panelMain.Width -
                                             sender.mainProgressPanel.Padding.Right -
                                             sender.panelMain.Margin.Right;
            sender.pbProgress.Width = sender.panelMain.Width -
                                      sender.mainProgressPanel.Padding.Right -
                                      sender.panelMain.Margin.Right -
                                      sender.pbProgress.Margin.Right;
            sender.gbSubTask.Width =
                sender.panelMain.Width - sender.gbSubTask.Padding.Right - sender.panelMain.Margin.Right;
            sender.subTaskPanel.Width = sender.gbSubTask.Width - sender.subTaskPanel.Padding.Right;
            sender.gbSubTask.Height = sender.panelMain.Height -
                                      sender.mainProgressPanel.Height -
                                      sender.gbSubTask.Padding.Bottom -
                                      sender.panelMain.Margin.Bottom;
            sender.subTaskPanel.Height = sender.gbSubTask.Height - sender.gbSubTask.Padding.Bottom;
        }

        private static void SetThreadTask(Task<ProgressIndicator> threadTask)
        {
            ThreadTask = threadTask;
        }


        public static void RunTask(
            Action<IProgressIndicator> task, Control parent, Action onWait,
            IProgressIndicator indicator)
        {
            Task t = new Task(() => task(indicator));
            t.Start();
            while (!t.IsCompleted)
            {
                onWait();
                if (ThreadTask != null && ThreadTask.Status != TaskStatus.Running && !ThreadTask.IsCompleted)
                {
                    ThreadTask.RunSynchronously();
                }
            }

            if (t.IsFaulted)
            {
                throw t.Exception.InnerException;
            }
        }


        public static (Form, Panel) CreateProgressForm()
        {
            Panel parent = CreateProgressPanel();
            ContainerForm c = new ContainerForm(
                                                parent,
                                                null,
                                                "Unpacking",
                                                SystemIcons.Application,
                                                FormBorderStyle.SizableToolWindow,
                                                new Size(1, 1),
                                                new Size(3000, 3000)
                                               );
            c.Show();
            c.Size = new Size(700, 600);
            return (c, parent);
        }

        private static Panel CreateProgressPanel()
        {
            Panel parent = new Panel();
            parent.Dock = DockStyle.Fill;
            return parent;
        }

        public static void RunTask(Action<IProgressIndicator> task, Control parent, Action onWait)
        {
            ProgressIndicator pi = CreateProgressIndicator(parent, SetThreadTask);
            RunTask(task, parent, onWait, pi);
        }

        public static void RunTask(Action<IProgressIndicator> task, Action onWait)
        {
            (Form c, Panel container) = CreateProgressForm();
            RunTask(task, container, onWait);
            c.Dispose();
        }

    }
}