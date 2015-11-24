using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using AlertDialog = Android.Support.V7.App.AlertDialog;

using Toolbar = Android.Support.V7.Widget.Toolbar;


using System;
using Android.Views;

namespace VisualCrypt
{
	[Activity (Label = "VisualCrypt", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : AppCompatActivity  // http://developer.android.com/reference/android/support/v7/app/AppCompatActivity.html
    {
		int count = 1;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

            // http://developer.android.com/training/appbar/index.html
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
         

            //Toolbar will now take on default actionbar characteristics
            SetSupportActionBar(toolbar);

            SupportActionBar.Title = "";


            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button> (Resource.Id.myButton);
			
			button.Click += delegate {
				button.Text = string.Format ("{0} clicks!", count++);
			};
		}

	    public override bool OnCreateOptionsMenu(IMenu menu)
	    {
            MenuInflater.Inflate(Resource.Menu.MenuMain, menu);
	        return base.OnCreateOptionsMenu(menu);
	    }


	    bool ShowDialog()
	    {
            var builder = new AlertDialog.Builder(this);

            builder.SetTitle("Hello Dialog")
                   .SetMessage("Is this material design?")
                   .SetPositiveButton("Yes", delegate { Console.WriteLine("Yes"); })
                   .SetNegativeButton("No", delegate { Console.WriteLine("No"); });

            builder.Create().Show();
	        return true;


	    }
    }
}


