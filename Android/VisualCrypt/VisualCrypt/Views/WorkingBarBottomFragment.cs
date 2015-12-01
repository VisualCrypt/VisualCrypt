using Android.OS;
using Android.Views;
using Android.Widget;
using VisualCrypt.Applications.Services.Interfaces;
using VisualCrypt.Language.Strings;
using Fragment = Android.Support.V4.App.Fragment;

namespace VisualCrypt.Views
{
    class WorkingBarBottomFragment : Fragment
    {
        readonly ResourceWrapper _resourceWrapper;
        public WorkingBarBottomFragment()
        {
            _resourceWrapper = Service.Get<ResourceWrapper>();
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.WorkingBarBottomFragment, container, false);
            view.FindViewById<TextView>(Resource.Id.workingBarBottomText).Text = _resourceWrapper.msgTakesTooLong;
            return view;
        }
    }
}