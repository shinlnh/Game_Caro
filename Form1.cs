using System;
using System.Drawing;
using System.Windows.Forms;

namespace Caro
{
    public partial class Form1 : Form
    {
        private const int GridSize = 20;
        private const int CellSize = 80;
        private char[,] board = new char[GridSize, GridSize];
        private char currentPlayer = 'X';

        private Label lblPlayerX;
        private Label lblPlayerO;
        private Label lblTimerX;
        private Label lblTimerO;
        private Button btnReset;

        private System.Windows.Forms.Timer turnTimer;
        private int timeLeft = 15;

        public Form1()
        {
            InitializeComponent();
            InitUI();
            InitBoard();
            UpdateTurnLabel();
            StartTimer();
        }

        private void InitUI()
        {
            this.Text = "Caro 9x9 - WinForms";

            int boardWidth = GridSize * CellSize;
            int boardHeight = GridSize * CellSize;
            int paddingRight = 200;

            this.Size = new Size(boardWidth + paddingRight, boardHeight + 40);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = true;
            this.DoubleBuffered = true;

            lblPlayerX = new Label()
            {
                Text = "Player X",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.Blue,
                AutoSize = true
            };
            this.Controls.Add(lblPlayerX);

            lblTimerX = new Label()
            {
                Text = "15s",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.Blue,
                AutoSize = true
            };
            this.Controls.Add(lblTimerX);

            lblPlayerO = new Label()
            {
                Text = "Player O",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true
            };
            this.Controls.Add(lblPlayerO);

            lblTimerO = new Label()
            {
                Text = "15s",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true
            };
            this.Controls.Add(lblTimerO);

            btnReset = new Button()
            {
                Text = "RESET",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Size = new Size(140, 50),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            btnReset.Click += btnReset_Click;
            this.Controls.Add(btnReset);

            void UpdateLayout()
            {
                int spacing = 30;
                int totalLabelWidth = lblPlayerX.Width + spacing + lblPlayerO.Width;
                int boardRight = GridSize * CellSize;
                int availableWidth = this.ClientSize.Width - boardRight;

                int labelsStartX = boardRight + (availableWidth - totalLabelWidth) / 2;
                int labelY = 30;

                lblPlayerX.Location = new Point(labelsStartX, labelY);
                lblTimerX.Location = new Point(lblPlayerX.Left, lblPlayerX.Bottom + 5);

                lblPlayerO.Location = new Point(lblPlayerX.Right + spacing, labelY);
                lblTimerO.Location = new Point(lblPlayerO.Left, lblPlayerO.Bottom + 5);

                btnReset.Location = new Point(
                    this.ClientSize.Width - btnReset.Width - 20,
                    this.ClientSize.Height - btnReset.Height - 20
                );
            }

            this.Load += (s, e) => UpdateLayout();
            this.Resize += (s, e) => UpdateLayout();

            turnTimer = new System.Windows.Forms.Timer();
            turnTimer.Interval = 1000;
            turnTimer.Tick += TurnTimer_Tick;
        }

        private void InitBoard()
        {
            for (int i = 0; i < GridSize; i++)
                for (int j = 0; j < GridSize; j++)
                    board[i, j] = '.';
        }

        private void StartTimer()
        {
            timeLeft = 15;
            UpdateTimerLabels();
            turnTimer.Start();
        }

        private void TurnTimer_Tick(object sender, EventArgs e)
        {
            timeLeft--;
            UpdateTimerLabels();

            if (timeLeft == 0)
            {
                turnTimer.Stop();
                MessageBox.Show($"Player {currentPlayer} ran out of time!\nPlayer {(currentPlayer == 'X' ? 'O' : 'X')} wins!", "Time Out");
                InitBoard();
                currentPlayer = 'X';
                UpdateTurnLabel();
                StartTimer();
                Invalidate();
            }
        }

        private void UpdateTimerLabels()
        {
            lblTimerX.Text = currentPlayer == 'X' ? $"{timeLeft}s" : "15s";
            lblTimerO.Text = currentPlayer == 'O' ? $"{timeLeft}s" : "15s";
        }

        private void UpdateTurnLabel()
        {
            if (currentPlayer == 'X')
            {
                lblPlayerX.ForeColor = Color.Blue;
                lblTimerX.ForeColor = Color.Blue;
                lblPlayerO.ForeColor = Color.Black;
                lblTimerO.ForeColor = Color.Black;
            }
            else
            {
                lblPlayerX.ForeColor = Color.Black;
                lblTimerX.ForeColor = Color.Black;
                lblPlayerO.ForeColor = Color.Blue;
                lblTimerO.ForeColor = Color.Blue;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            for (int i = 0; i <= GridSize; i++)
            {
                g.DrawLine(Pens.Black, 0, i * CellSize, GridSize * CellSize, i * CellSize);
                g.DrawLine(Pens.Black, i * CellSize, 0, i * CellSize, GridSize * CellSize);
            }

            Font font = new Font("Arial", 28, FontStyle.Bold);
            StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    if (board[i, j] != '.')
                    {
                        Rectangle rect = new Rectangle(j * CellSize, i * CellSize, CellSize, CellSize);
                        g.DrawString(board[i, j].ToString(), font, Brushes.Blue, rect, sf);
                    }
                }
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.X >= GridSize * CellSize || e.Y >= GridSize * CellSize) return;

            int row = e.Y / CellSize;
            int col = e.X / CellSize;

            if (row >= GridSize || col >= GridSize) return;

            if (board[row, col] == '.')
            {
                board[row, col] = currentPlayer;

                if (CheckWin(row, col))
                {
                    turnTimer.Stop();
                    Invalidate();
                    MessageBox.Show($"Player {currentPlayer} wins!", "Game Over");
                    InitBoard();
                    currentPlayer = 'X';
                    UpdateTurnLabel();
                    StartTimer();
                }
                else
                {
                    currentPlayer = currentPlayer == 'X' ? 'O' : 'X';
                    UpdateTurnLabel();
                    StartTimer();
                }
                Invalidate();
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            InitBoard();
            currentPlayer = 'X';
            UpdateTurnLabel();
            StartTimer();
            Invalidate();
        }

        private bool CheckWin(int row, int col)
        {
            return CheckDirection(row, col, 1, 0)
                || CheckDirection(row, col, 0, 1)
                || CheckDirection(row, col, 1, 1)
                || CheckDirection(row, col, 1, -1);
        }

        private bool CheckDirection(int row, int col, int dRow, int dCol)
        {
            char symbol = board[row, col];
            int count = 1;

            for (int i = 1; i < 5; i++)
            {
                int r = row + dRow * i;
                int c = col + dCol * i;
                if (r >= 0 && r < GridSize && c >= 0 && c < GridSize && board[r, c] == symbol)
                    count++;
                else break;
            }

            for (int i = 1; i < 5; i++)
            {
                int r = row - dRow * i;
                int c = col - dCol * i;
                if (r >= 0 && r < GridSize && c >= 0 && c < GridSize && board[r, c] == symbol)
                    count++;
                else break;
            }

            return count >= 5;
        }
    }
}