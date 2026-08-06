using System;
using System.Drawing;
using System.Windows.Forms;

namespace LotTraceApp.Forms
{
    public sealed class ProgressForm : Form
    {
        private readonly Label _messageLabel;
        private readonly ProgressBar _progressBar;
        private readonly Button _cancelButton;

        public event EventHandler? CancelRequested;
        public bool IsCancellationRequested { get; private set; }

        public ProgressForm(string title)
        {
            Text = string.IsNullOrWhiteSpace(title) ? "\u51e6\u7406\u4e2d" : title;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(420, 156);
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            ControlBox = false;
            Font = new Font("MS UI Gothic", 10F, FontStyle.Regular, GraphicsUnit.Point, 128);

            _messageLabel = new Label
            {
                AutoSize = false,
                Location = new Point(20, 18),
                Size = new Size(380, 28),
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "\u51e6\u7406\u3092\u958b\u59cb\u3057\u3066\u3044\u307e\u3059..."
            };

            _progressBar = new ProgressBar
            {
                Location = new Point(20, 60),
                Size = new Size(380, 24),
                Style = ProgressBarStyle.Continuous,
                Minimum = 0,
                Maximum = 100,
                Value = 0
            };

            _cancelButton = new Button
            {
                Location = new Point(310, 102),
                Size = new Size(90, 30),
                Text = "キャンセル",
                UseVisualStyleBackColor = true
            };
            _cancelButton.Click += CancelButton_Click;

            Controls.Add(_messageLabel);
            Controls.Add(_progressBar);
            Controls.Add(_cancelButton);
        }

        private void CancelButton_Click(object? sender, EventArgs e)
        {
            if (IsCancellationRequested)
                return;

            IsCancellationRequested = true;
            _cancelButton.Enabled = false;
            SetMessage("キャンセルしています...");

            var handler = CancelRequested;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public void SetMessage(string message)
        {
            SetProgress(message, null);
        }

        public void SetProgress(string message, int? percent)
        {
            if (IsDisposed)
                return;

            _messageLabel.Text = string.IsNullOrWhiteSpace(message)
                ? "\u51e6\u7406\u4e2d\u3067\u3059..."
                : message;

            if (!percent.HasValue)
                return;

            int value = Math.Max(_progressBar.Minimum, Math.Min(_progressBar.Maximum, percent.Value));
            _progressBar.Value = value;
        }
    }
}
