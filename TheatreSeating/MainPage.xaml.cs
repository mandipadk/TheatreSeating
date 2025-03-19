using System.Collections.Specialized;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using System.Transactions;
using AuthenticationServices;
using SharedWithYouCore;

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
                            // added check for reserved seats
                            if(seatingChart[i, j].Reserved)
                            {
                                await DisplayAlert("Error", "Seat is already reserved.", "Ok");
                                return;
                            }
                            seatingChart[i, j].Reserved = true;
                            await DisplayAlert("Successfully Reserved", "Your seat was reserved successfully!", "Ok");
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

        //Nancy Neria
        private void ButtonCancelReservation(object sender, EventArgs e)
        {
            var seat = await DisplayPromptAsync("Cancel Seat Number", "Enter seat number: ");

            if (seat != null)
            {
                for (int i = 0; i < seatingChart.GetLength(0); i++)
                {
                    for (int j = 0; j < seatingChart.GetLength(1); j++)
                    {
                        if (seatingChart[i, j].Name == seat)
                        {
                            // added check for reserved seats
                            if (seatingChart[i, j].Reserved == false)
                            {
                                await DisplayAlert("Error", "Seat is not reserved", "Ok");
                                return;
                            }
                            seatingChart[i, j].Reserved = false;
                            await DisplayAlert("Successfully Cancelled", "Your seat was canceled successfully!", "Ok");
                            RefreshSeating();
                            return;
                        }
                    }
                }
            
                await DisplayAlert("Error", "Seat was not found.", "Ok");
            }
        }

        //Assign to: Dina Hadwan, w10166376
        private async void ButtonCancelReservationRange(object sender, EventArgs e)
        {
            // Get user input
            var input = await DisplayPromptAsync("Enter Seat Range", "Enter starting and ending seat (ex. A1:A4):");

            if (input != null)
            {
                string[] range = input.Split(':');

                // Checks valid range
                if (range.Length != 2)
                {
                    await DisplayAlert("Error", "Invalid range format.", "Ok");
                    return;
                }

                string startSeat = range[0];
                string endSeat = range[1];

                // Converts letter row to numerical row
                int startRow = startSeat[0] - 'A';
                int endRow = endSeat[0] - 'A';

                
                // Makes sure both seats are in same row
                if (startRow != endRow)
                {
                    await DisplayAlert("Error", "Seats have to be in same row.", "Ok");
                    return;
                }

                // Convert from string to integer
                int startCol = int.Parse(startSeat.Substring(1)) - 1;
                int endCol = int.Parse(endSeat.Substring(1)) - 1;


                // Checks seat range 
                if (startCol > endCol || startCol < 0 || endCol >= seatingChart.GetLength(1))
                {
                    await DisplayAlert("Error", "Invalid seat range.", "Ok");
                    return;
                }

                // Checks if all seats are reserved before it cancels
                bool anyNotReserved = false;
                for(int col = startCol; col <= endCol; col++)
                {
                    if (!seatingChart[startRow, col].Reserved)
                    {
                        anyNotReserved = true;
                        break;
                    }
                }

                if (anyNotReserved)
                {
                    await DisplayAlert("Error", "One or more seats not reserved.", "Ok");
                    return;

                }

                // Cancel the reservation seats
                for(int col = startCol; col <= endCol; col++)
                {
                    seatingChart[startRow, col].Reserved = false;
                }

                await DisplayAlert("Sucess", "Seats reservation successfully canceled.", "Ok");

                RefreshSeating();


            }
        }

        //Assigned to Rashika Karmacharya, w10172109 and Ethan Carolina, W10126539
        private async void ButtonResetSeatingChart(object sender, EventArgs e)
        {
            bool confirmReset = await DisplayAlert("Confirm Reset", "Are you sure you want to reset all seat reservations?", "Yes", "No");

            if (confirmReset)
            {
                for (int row = 0; row < 5; row++)
                {
                    for (int col = 0; col < 10; col++)
                    {
                        seatingChart[row, col].Reserved = false;
                    }
                }

                await DisplayAlert("Reset", "All seat reservations have been cleared.", "Ok");
                RefreshSeating();
            }
        }

    }

}
