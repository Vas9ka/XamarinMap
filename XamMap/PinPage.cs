using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Essentials;
using Xamarin.Forms.Internals;
using System;
using BetterGeolocator;
using System.Collections.Generic;

namespace XamMap
{
    public class PinPage : ContentPage
    {
        Xamarin.Forms.Maps.Map map;

        public PinPage()
        {
            map = new Xamarin.Forms.Maps.Map
            {
                IsShowingUser = true,
                HeightRequest = 100,
                WidthRequest = 960,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            map.MoveToRegion(MapSpan.FromCenterAndRadius(
                new Position(59.939042, 30.315799), Distance.FromMiles(3)));
            map.MapClicked += (sender, e) =>
            {
                map.Pins.Add(new Pin
                {
                    Position = new Position(e.Position.Latitude,e.Position.Longitude),
                    Label = "Marker"
                });
            };
            var CheckDistance = new Button { Text = "Calculate distance" };
            CheckDistance.Clicked += (sender, e) =>
            {
                var locator = new Geolocator();
                async void GetLocation()
                {
                    var loc = await locator.GetLocation(TimeSpan.FromSeconds(30), 200);
                    var userLat = loc.Coordinate.Latitude;
                    var userLong = loc.Coordinate.Longitude;
                    foreach (var pin in map.Pins)
                    {
                        double distance = Location.CalculateDistance(pin.Position.Latitude, pin.Position.Longitude, userLat, userLong, DistanceUnits.Kilometers);
                        if (distance < 2)
                        {
                            async void SendMail()
                            {
                                var Entry = new Entry { Text = "Enter your email" };
                                List<string> to = new List<string>();
                                to.Add(Entry.Text);
                                var message = new EmailMessage
                                {
                                    Subject = "Point",
                                    Body = "User coordinates - " + userLat + " " + userLong
                                    + "\n" + "Point coordinates - " + pin.Position.Latitude + " " + pin.Position.Longitude,
                                    To = to
                                };
                                await Email.ComposeAsync(message);

                            }
                            SendMail();
                        }
                    }

                }

                GetLocation();
            };

            var delPins = new Button { Text = "Delete pins" };
            delPins.Clicked += (sender, e) => {
                map.Pins.Clear();

            };
            

           

            Content = new StackLayout
            {
                Spacing = 0,
                Children = {
                    map,
                   delPins,
                   CheckDistance
                }
            };
        }
    }
}