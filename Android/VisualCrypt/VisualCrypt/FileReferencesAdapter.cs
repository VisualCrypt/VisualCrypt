using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using VisualCrypt.Applications.Models;

namespace VisualCrypt
{
    class FileReferencesAdapter : BaseAdapter<FileReference>
    {
        Activity context = null;
        IList<FileReference> tasks = new List<FileReference>();

        public FileReferencesAdapter(Activity context, IList<FileReference> tasks) : base ()
		{
            this.context = context;
            this.tasks = tasks;
        }

        public override FileReference this[int position]
        {
            get { return tasks[position]; }
        }

        public override int Count
        {
            get { return tasks.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            // Get our object for position
            var item = tasks[position];

            //Try to reuse convertView if it's not  null, otherwise inflate it from our item layout
            // gives us some performance gains by not always inflating a new view
            // will sound familiar to MonoTouch developers with UITableViewCell.DequeueReusableCell()

            //			var view = (convertView ?? 
            //					context.LayoutInflater.Inflate(
            //					Resource.Layout.TaskListItem, 
            //					parent, 
            //					false)) as LinearLayout;
            //			// Find references to each subview in the list item's view
            //			var txtName = view.FindViewById<TextView>(Resource.Id.NameText);
            //			var txtDescription = view.FindViewById<TextView>(Resource.Id.NotesText);
            //			//Assign item's values to the various subviews
            //			txtName.SetText (item.Name, TextView.BufferType.Normal);
            //			txtDescription.SetText (item.Notes, TextView.BufferType.Normal);

            // TODO: use this code to populate the row, and remove the above view
            var view = (convertView ??
                context.LayoutInflater.Inflate(
                    Android.Resource.Layout.SimpleListItemChecked,
                    parent,
                    false)) as CheckedTextView;
            view.SetText(item.ShortFilename == "" ? "Untitled.visualcrypt" : item.ShortFilename, TextView.BufferType.Normal);
            view.Checked = true;


            //Finally return the view
            return view;
        }
    }
}