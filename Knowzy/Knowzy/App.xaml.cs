using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;



using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using System.Threading.Tasks;

#if !WINDOWS_UWP
using Microsoft.Azure.Mobile.Distribute;
#endif 

namespace Knowzy
{
	public partial class App : Application
	{
        public App( Dictionary <string, string> parameters = null  )
        {
            InitializeComponent();

            string task = string.Empty;
            string noseId = string.Empty;
            if (parameters != null)
            {
                parameters.TryGetValue(Constants.TaskParam, out task);
                parameters.TryGetValue(Constants.NoseParam, out noseId);
            } 
                         
            if (task == Constants.CaptureImageTask  )
            {
                MainPage = new NavigationPage(new CameraPage(noseId)); 
            } 
            else
                MainPage = new NavigationPage(new MainPage());

        }


		protected override void OnStart ()
		{


#if !WINDOWS_UWP && !__ANDROID__
            Distribute.DontCheckForUpdatesInDebug();
#endif

#if !WINDOWS_UWP 
            Distribute.ReleaseAvailable = OnReleaseAvailable;
#endif

            string secret = Xamarin.Forms.Device.OnPlatform<string>("1ed68692-9324-44c3-8088-989287af52ba", "8ed7d024-dd9e-4f33-8674-ad5f2e684472", "d4db4baf-e20e-4ea9-b788-6cd572a796d7" );
            
            MobileCenter.Start(secret ,
                   typeof(Analytics)
#if !WINDOWS_UWP 
                   , typeof(Crashes)
                   , typeof(Distribute) 
                   
#endif                    
                   );             
        }



#if !WINDOWS_UWP 
        bool OnReleaseAvailable(ReleaseDetails releaseDetails)
        {
            // Look at releaseDetails public properties to get version information, release notes text or release notes URL
            string versionName = releaseDetails.ShortVersion;
            string versionCodeOrBuildNumber = releaseDetails.Version;
            string releaseNotes = releaseDetails.ReleaseNotes;
            Uri releaseNotesUrl = releaseDetails.ReleaseNotesUrl;

            // custom dialog
            var title = "Version " + versionName + " available!";
            Task answer;

            // On mandatory update, user cannot postpone
            if (releaseDetails.MandatoryUpdate)
            {
                answer = Current.MainPage.DisplayAlert(title, releaseNotes, "Download and Install");
            }
            else
            {
                answer = Current.MainPage.DisplayAlert(title, releaseNotes, "Download and Install", "Maybe tomorrow...");
            }
            answer.ContinueWith((task) =>
            {
                // If mandatory or if answer was positive
                if (releaseDetails.MandatoryUpdate || (task as Task<bool>).Result)
                {
                    // Notify SDK that user selected update
                    Distribute.NotifyUpdateAction(UpdateAction.Update);
                }
                else
                {
                    // Notify SDK that user selected postpone (for 1 day)
                    // Note that this method call is ignored by the SDK if the update is mandatory
                    Distribute.NotifyUpdateAction(UpdateAction.Postpone);
                }
            });

            // Return true if you are using your own dialog, false otherwise
            return true;
        }
#endif 


        protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
