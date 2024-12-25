using System;
using System.Drawing;
using System.Windows.Forms;

namespace MayinTarlasi
{
    public partial class Form1 : Form
    {
        private const int GridSize = 30; // Oyun ızgarasının boyutunu sabit olarak tanımlar. Bu, 30x30 hücrelik bir ızgara anlamına gelir.
        private int mineCount;  // Toplam mayın sayısını tutmak için değişken. Bu, oyun zorluğunu belirlemek için kullanılır.
        private Button[,] buttons; //Izgaradaki düğmeleri (hücreleri) temsil eden 2 boyutlu bir dizi.
        private bool[,] mines; // true: Hücrede mayın var, false: Hücrede mayın yok.
        private bool[,] revealed; // true: Hücre zaten açılmış, false: Hücre henüz açılmamış.
        private int cellsRevealed; //  Oyunun kazanılıp kazanılmadığını kontrol etmek için kullanılır.

        public Form1(int mineCount)
        {
            this.mineCount = mineCount; // Oyundaki toplam mayın sayısını kaydeder.
            InitializeComponent();
            InitializeGame();// Oyunun başlangıç durumunu hazırlar.
        }


        private void InitializeGame()
        {
            this.Text = "Mayın Tarlası";// Form başlığını ayarlar.
            this.ClientSize = new Size(800, 850);// Formun boyutlarını belirler.
            this.StartPosition = FormStartPosition.CenterScreen;// Formun ekranın ortasında açılmasını sağlar.

            //  Reset button
            Button resetButton = new Button
            {
                Text = "Reset",
                Font = new Font("Arial", 12, FontStyle.Bold),// Yazı tipi ve boyutu.
                Size = new Size(100, 40),// Butonun boyutları.
                Location = new Point((this.ClientSize.Width - 100) / 2, 10)// Butonun form üzerindeki konumu
            };
            resetButton.Click += ResetButton_Click; // Reset butonuna tıklandığında yapılacak işlem atanır.
            this.Controls.Add(resetButton);  // Reset butonu form üzerine eklenir.

            // Oyun ızgarası için gerekli değişkenler hazırlanır.
            buttons = new Button[GridSize, GridSize];// Tüm hücreleri temsil eden düğmeler
            mines = new bool[GridSize, GridSize]; ; // Mayınların bulunduğu hücreler.
            revealed = new bool[GridSize, GridSize];// Açılan hücrelerin durumları.
            cellsRevealed = 0; // Açılan hücrelerin sayısını tutar.

            int buttonSize = 25;// Her hücrenin boyutu.
            int startY = resetButton.Bottom + 10;// Izgaranın başlangıç y koordinatı

            // Izgaradaki düğmeler oluşturulur ve form üzerine eklenir.
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    buttons[i, j] = new Button
                    {
                        Size = new Size(buttonSize, buttonSize), // Hücre boyutları.
                        Location = new Point(j * buttonSize, startY + i * buttonSize),// Hücrenin konumu
                        BackColor = Color.LightGray, // Hücrenin başlangıç rengi.
                        Tag = new Point(i, j) // Hücrenin koordinatları.
                    };

                    buttons[i, j].Click += Button_Click;// Hücreye tıklandığında yapılacak işlem atanır.
                    this.Controls.Add(buttons[i, j]);// Hücre form üzerine eklenir.
                }
            }

            PlaceMines(); // Mayınlar rastgele yerlere yerleştirilir
        }

        // Mayınları rastgele hücrelere yerleştirir.
        private void PlaceMines()
        {
            Random random = new Random(); // Rastgele sayı üretici.
            int minesPlaced = 0;// Yerleştirilen mayın sayısı.

            while (minesPlaced < mineCount)// Belirtilen sayıda mayın yerleştirilene kadar devam eder.
            {
                int row = random.Next(GridSize); // Rastgele bir satır seçilir.
                int col = random.Next(GridSize);// Rastgele bir sütun seçilir.

                if (!mines[row, col]) // Avoid placing multiple mines in the same spot
                {
                    mines[row, col] = true; // Mayın yerleştirilir.
                    minesPlaced++;// Yerleştirilen mayın sayısı artırılır.
                }
            }
        }


        private void Button_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button; // Tıklanan buton referansı.
            Point location = (Point)clickedButton.Tag;// Tıklanan hücrenin koordinatları.
            int row = location.X; // Satır koordinatı.
            int col = location.Y;// Sütun koordinatı.

            if (mines[row, col])// Eğer tıklanan hücrede mayın varsa...
            {
                clickedButton.BackColor = Color.Red; // Hücrenin rengi kırmızıya döner.
                clickedButton.Text = "X";// Hücreye "X" yazılır.
                RevealMines();// Tüm mayınlar gösterilir.
                MessageBox.Show("Game Over!  Bitti", "Game Over");// Oyunun bittiği bildirilir
                ReturnToForm2(); // Ana menüye dönülür.
            }
            else
            {
                RevealSafeCells(row, col); // Güvenli hücreler açılır.
                if (cellsRevealed == (GridSize * GridSize - mineCount)) // Tüm güvenli hücreler açıldıysa.
                {
                    MessageBox.Show("Congratulations! You win.", "You Win");// Oyunun kazanıldığı bildirilir
                    ReturnToForm2();//Ana menüye Dönülür.
                }
            }
        }
        // Tüm mayınları gösterir.
        private void RevealMines()
        {
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    if (mines[i, j])// Eğer hücrede mayın varsa
                    {
                        buttons[i, j].BackColor = Color.Red;// Hücre kırmızıya boyanır
                        buttons[i, j].Text = "X";//Hücrede X yazolır
                    }
                }
            }
        }
        // Güvenli hücreleri açar ve varsa komşu güvenli hücreleri de açar.
        private void RevealSafeCells(int row, int col)
        {
            if (row < 0 || col < 0 || row >= GridSize || col >= GridSize || revealed[row, col]) return;// Geçersiz koordinatlar veya zaten açılmış hücreler için işlemi durdur.

            revealed[row, col] = true; // Hücre açıldı olarak işaretlenir.
            cellsRevealed++; // Açılan hücre sayısı artırılır.

            int adjacentMines = CountAdjacentMines(row, col); // Hücrenin etrafındaki mayınları sayar.
            buttons[row, col].BackColor = Color.White;// Hücrenin rengi beyaza döner.
            buttons[row, col].Text = adjacentMines > 0 ? adjacentMines.ToString() : "";// Komşu mayın sayısını gösterir.

            if (adjacentMines == 0)// Eğer hücrenin etrafında mayın yoksa...
            {
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        RevealSafeCells(row + i, col + j);// Komşu hücreleri de aç.
                    }
                }
            }
        }
        // Hücrenin etrafındaki mayınları sayar.
        private int CountAdjacentMines(int row, int col)
        {
            int count = 0;   // Komşu mayın sayısı.

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int newRow = row + i;
                    int newCol = col + j;

                    if (newRow >= 0 && newRow < GridSize && newCol >= 0 && newCol < GridSize && mines[newRow, newCol])
                    {
                        count++; // Komşuda mayın varsa sayacı artır.
                    }
                }
            }

            return count;
        }
        // Reset butonuna tıklandığında ana menüye döner.
        private void ResetButton_Click(object sender, EventArgs e)
        {
            ReturnToForm2();// Ana menüye dön.
        }
        // Oyunun bittiğinde ana menüye dönüş işlemleri
        private void ReturnToForm2()
        {
            this.Hide(); //  Geçerli formu gizler.
            Form2 form2 = new Form2();// Yeni bir Form2 oluşturur.
            if (form2.ShowDialog() == DialogResult.OK) // Eğer Form2'den olumlu bir yanıt alınırsa.
            {
                this.Close(); //  Geçerli formu kapatır.
                Application.Run(new Form1(form2.MineCount)); // Yeni bir oyun başlatır.
            }
        }
    }
}
