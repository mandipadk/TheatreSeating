using System.Collections.Specialized;
using System.Reflection.Metadata;

namespace TheatreSeating
{
    public class SeatingUnit
    {
        public string Name { get; set; }
        public bool Reserved { get; set; }

        public SeatingUnit(string name, bool reserved = false)
        {
            Name = name;
            Reserved = reserved;
        }

    }

    public partial class MainPage : ContentPage
    {
        SeatingUnit[,] seatingChart = new SeatingUnit[5, 10];

        public MainPage()
        {
            InitializeComponent();
            GenerateSeatingNames();
            RefreshSeating();
        }

        private async void ButtonReserveSeat(object sender, EventArgs e)
        {
            var seat = await DisplayPromptAsync("Enter Seat Number", "Enter seat number: ");

            if (seat != null)
            {
                for (int i = 0; i < seatingChart.GetLength(0); i++)
                {
                    for (int j = 0; j < seatingChart.GetLength(1); j++)
                    {
                        if (seatingChart[i, j].Name == seat)
                        {
                            seatingChart[i, j].Reserved = true;
                            await DisplayAlert("Successfully Reserverd", "Your seat was reserverd successfully!", "Ok");
                            RefreshSeating();
                            return;
                        }
                    }
                }

                await DisplayAlert("Error", "Seat was not found.", "Ok");
            }
        }

        private void GenerateSeatingNames()
        {
            List<string> letters = new List<string>();
            for (char c = 'A'; c <= 'Z'; c++)
            {
                letters.Add(c.ToString());
            }

            int letterIndex = 0;

            for (int row = 0; row < seatingChart.GetLength(0); row++)
            {
                for (int column = 0; column < seatingChart.GetLength(1); column++)
                {
                    seatingChart[row, column] = new SeatingUnit(letters[letterIndex] + (column + 1).ToString());
                }

                letterIndex++;
            }
        }

        private void RefreshSeating()
        {
            grdSeatingView.RowDefinitions.Clear();
            grdSeatingView.ColumnDefinitions.Clear();
            grdSeatingView.Children.Clear();

            for (int row = 0; row < seatingChart.GetLength(0); row++)
            {
                var grdRow = new RowDefinition();
                grdRow.Height = 50;

                grdSeatingView.RowDefinitions.Add(grdRow);

                for (int column = 0; column < seatingChart.GetLength(1); column++)
                {
                    var grdColumn = new ColumnDefinition();
                    grdColumn.Width = 50;

                    grdSeatingView.ColumnDefinitions.Add(grdColumn);

                    var text = seatingChart[row, column].Name;

                    var seatLabel = new Label();
                    seatLabel.Text = text;
                    seatLabel.HorizontalOptions = LayoutOptions.Center;
                    seatLabel.VerticalOptions = LayoutOptions.Center;
                    seatLabel.BackgroundColor = Color.Parse("#333388");
                    seatLabel.Padding = 10;

                    if (seatingChart[row, column].Reserved == true)
                    {
                        //Change the color of this seat to represent its reserved.
                        seatLabel.BackgroundColor = Color.Parse("#883333");

                    }

                    Grid.SetRow(seatLabel, row);
                    Grid.SetColumn(seatLabel, column);
                    grdSeatingView.Children.Add(seatLabel);

                }
            }
        }

        //Assigned to: Mandip Adhikari, w10167734
        private async void ButtonReserveRange(object sender, EventArgs e)
        {   
            // Get input from user
            var input = await DisplayPromptAsync("Enter Seat Range", "Enter starting and ending seat (e.g., A1:A4):");

            if (input != null)
            {
                string[] range = input.Split(':');
                // checks if a valid range was entered
                if (range.Length != 2)
                {
                    await DisplayAlert("Error", "Invalid range format.", "Ok");
                    return;
                }

                string startSeat = range[0];
                string endSeat = range[1];
                // convert letter row into numeric row
                int startRow = startSeat[0] - 'A';
                int endRow = endSeat[0] - 'A';

                if (startRow != endRow)
                {
                    await DisplayAlert("Error", "Seats must be in the same row.", "Ok");
                    return;
                }

                int startCol = int.Parse(startSeat.Substring(1)) - 1;
                int endCol = int.Parse(endSeat.Substring(1)) - 1;

                if (startCol > endCol || startCol < 0 || endCol >= seatingChart.GetLength(1))
                {
                    await DisplayAlert("Error", "Invalid seat range.", "Ok");
                    return;
                }
                // checks if any seats in the range are already reserved
                for (int col = startCol; col <= endCol; col++)
                {
                    if (seatingChart[startRow, col].Reserved)
                    {
                        await DisplayAlert("Error", "One or more seats are already reserved.", "Ok");
                        return;
                    }
                }

                for (int col = startCol; col <= endCol; col++)
                {
                    seatingChart[startRow, col].Reserved = true;
                }

                await DisplayAlert("Success", "Seats reserved successfully!", "Ok");
                RefreshSeating();
            }
        }

        //Assign to Team 2 Member
        private void ButtonCancelReservation(object sender, EventArgs e)
        {

        }

        //Assign to Team 3 Member
        private void ButtonCancelReservationRange(object sender, EventArgs e)
        {

        }

        //Assign to Team 4 Member
        private void ButtonResetSeatingChart(object sender, EventArgs e)
        {

        }
    }

}