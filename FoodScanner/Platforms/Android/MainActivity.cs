using Android.App;
using Android.Content.PM;
using Android.OS;

namespace FoodScanner
{
    [Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    ConfigurationChanges =
        ConfigChanges.ScreenSize |
        ConfigChanges.Orientation |
        ConfigChanges.UiMode |
        ConfigChanges.ScreenLayout |
        ConfigChanges.SmallestScreenSize |
        ConfigChanges.Density
)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(
            Android.OS.Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override void OnRequestPermissionsResult(
            int requestCode,
            string[] permissions,
            Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(
                requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(
                requestCode, permissions, grantResults);
        }
    }
}
