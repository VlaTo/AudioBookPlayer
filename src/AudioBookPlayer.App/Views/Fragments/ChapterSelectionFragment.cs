#nullable enable

using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Support.V4.Media.Session;
using Android.Views;
using Android.Widget;
using AndroidX.Core.View;
using AndroidX.RecyclerView.Widget;
using AudioBookPlayer.App.Core.Extensions;
using Google.Android.Material.BottomAppBar;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.Shape;

namespace AudioBookPlayer.App.Views.Fragments
{
    // https://medium.com/snapp-mobile/draggable-bottom-navigation-drawer-c56aba594f1e
    // https://habr.com/ru/post/567828/
    public class ChapterSelectionFragment : BottomSheetDialogFragment
    {
        private const string QueueArgumentName = "Queue.Items";

        private RecyclerView? recyclerView;

        public IList<MediaSessionCompat.QueueItem>? Queue => Arguments?.GetQueue(QueueArgumentName);

        public static ChapterSelectionFragment NewInstance(IList<MediaSessionCompat.QueueItem> queue)
        {
            var bundle = new Bundle();
            
            bundle.PutQueue(QueueArgumentName, queue);

            return new ChapterSelectionFragment
            {
                Arguments = bundle
            };
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var dialog = base.OnCreateDialog(savedInstanceState);

            if (dialog is BottomSheetDialog bottom)
            {
                bottom.Behavior.AddBottomSheetCallback(new MyBottomSheetBehaviorCallback());
                bottom.Behavior.SetPeekHeight(140, true);
                bottom.Behavior.Hideable = true;
                //bottom.Behavior.HalfExpandedRatio = 0.65f;
            }

            return dialog;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var _ = base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.fragment_chapter_selection, container, false);

            if (null != view)
            {
                recyclerView = view.FindViewById<RecyclerView>(Resource.Id.list_view_recycler);
            }

            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            if (null != recyclerView)
            {
                recyclerView.SetLayoutManager(new LinearLayoutManager(Application.Context));
                recyclerView.SetAdapter(new ItemAdapter(Queue));
            }
        }

        //
        private sealed class MyBottomSheetBehaviorCallback : BottomSheetBehavior.BottomSheetCallback
        {
            public override void OnSlide(View bottomSheet, float newState)
            {
                //System.Diagnostics.Debug.WriteLine($"[MyBottomSheetBehaviorCallback.OnSlide] newState: {newState}");
            }

            public override void OnStateChanged(View bottomSheet, int newState)
            {
                switch (newState)
                {
                    case BottomSheetBehavior.StateDragging:
                    {
                        // System.Diagnostics.Debug.WriteLine("[MyBottomSheetBehaviorCallback.OnStateChanged] State = BottomSheetBehavior.StateDragging");
                        break;
                    }

                    case BottomSheetBehavior.StateSettling:
                    {
                        // System.Diagnostics.Debug.WriteLine("[MyBottomSheetBehaviorCallback.OnStateChanged] State = BottomSheetBehavior.StateSettling");
                        break;
                    }

                    case BottomSheetBehavior.StateExpanded:
                    {
                        System.Diagnostics.Debug.WriteLine("[MyBottomSheetBehaviorCallback.OnStateChanged] State = BottomSheetBehavior.StateExpanded");
                        //In the EXPANDED STATE apply a new MaterialShapeDrawable with rounded cornes
                        var newMaterialShapeDrawable = CreateMaterialShapeDrawable(bottomSheet);
                        ViewCompat.SetBackground(bottomSheet, newMaterialShapeDrawable);

                        break;
                    }

                    case BottomSheetBehavior.StateCollapsed:
                    {
                        System.Diagnostics.Debug.WriteLine("[MyBottomSheetBehaviorCallback.OnStateChanged] State = BottomSheetBehavior.StateCollapsed");
                        break;
                    }

                    case BottomSheetBehavior.StateHidden:
                    {
                        System.Diagnostics.Debug.WriteLine("[MyBottomSheetBehaviorCallback.OnStateChanged] State = BottomSheetBehavior.StateHidden");
                        break;
                    }

                    case BottomSheetBehavior.StateHalfExpanded:
                    {
                        System.Diagnostics.Debug.WriteLine("[MyBottomSheetBehaviorCallback.OnStateChanged] State = BottomSheetBehavior.StateHalfExpanded");
                        break;
                    }
                }
            }

            private static MaterialShapeDrawable CreateMaterialShapeDrawable(View bottomSheet)
            {
                var cornerSize = new AbsoluteCornerSize(54.0f);
                var shapeAppearanceModel = new ShapeAppearanceModel.Builder()
                    .SetTopLeftCornerSize(cornerSize)
                    .SetTopRightCornerSize(cornerSize)
                    .Build();

                //( Context, 0, Resource.Style.CustomShapeAppearanceBottomSheetDialog)
                //        .build();

                //Create a new MaterialShapeDrawable (you can't use the original MaterialShapeDrawable in the BottoSheet)
                var currentShape = (MaterialShapeDrawable)bottomSheet.Background;
                var shape = new MaterialShapeDrawable(shapeAppearanceModel);

                //Copy the attributes in the new MaterialShapeDrawable
                shape.InitializeElevationOverlay(Application.Context);
                shape.SetTintList(currentShape.TintList);
                shape.FillColor = currentShape.FillColor;
                shape.Elevation = currentShape.Elevation;
                shape.StrokeWidth = currentShape.StrokeWidth;
                shape.StrokeColor = currentShape.StrokeColor;

                return shape;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class ViewHolder : RecyclerView.ViewHolder
        {
            private readonly TextView? textView;

            public ViewHolder(View? itemView)
                : base(itemView)
            {
                textView = itemView?.FindViewById<TextView>(Resource.Id.text);
            }

            public void SetTitle(string value)
            {
                if (null != textView)
                {
                    textView.Text = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private sealed class ItemAdapter : RecyclerView.Adapter
        {
            private readonly IList<MediaSessionCompat.QueueItem>? queue;

            public override int ItemCount => (queue?.Count ?? 0) + 10;

            public ItemAdapter(IList<MediaSessionCompat.QueueItem>? queue)
            {
                this.queue = queue;
            }

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                if (null != queue && holder is ViewHolder textView)
                {
                    var index = position % queue.Count;
                    textView.SetTitle(queue[index].Description.Title);
                }
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                var layoutInflater = LayoutInflater.From(parent.Context);
                var itemView = layoutInflater?.Inflate(Resource.Layout.content_chapter_selection_item, parent, false);

                return new ViewHolder(itemView);
            }
        }
    }
}