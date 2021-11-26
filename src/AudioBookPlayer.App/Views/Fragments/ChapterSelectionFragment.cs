#nullable enable

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Core.View;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.Shape;

namespace AudioBookPlayer.App.Views.Fragments
{
    public class ChapterSelectionFragment : BottomSheetDialogFragment
    {
        private RecyclerView? recyclerView;

        public int Count => Arguments?.GetInt("Chapters.Count", 0) ?? 0;

        public static ChapterSelectionFragment NewInstance(int count)
        {
            var bundle = new Bundle();

            bundle.PutInt("Chapters.Count", count);

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
                recyclerView.SetAdapter(new ItemAdapter(10));
            }
        }

        //
        private sealed class MyBottomSheetBehaviorCallback : BottomSheetBehavior.BottomSheetCallback
        {
            public override void OnSlide(View bottomSheet, float newState)
            {
            }

            public override void OnStateChanged(View bottomSheet, int newState)
            {
                if (BottomSheetBehavior.StateExpanded == newState)
                {
                    //In the EXPANDED STATE apply a new MaterialShapeDrawable with rounded cornes
                    MaterialShapeDrawable newMaterialShapeDrawable = CreateMaterialShapeDrawable(bottomSheet);
                    ViewCompat.SetBackground(bottomSheet, newMaterialShapeDrawable);
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

        //
        private sealed class ViewHolder : RecyclerView.ViewHolder
        {
            public TextView? TextView
            {
                get;
            }

            public ViewHolder(View? itemView)
                : base(itemView)
            {
                TextView = itemView?.FindViewById<TextView>(Resource.Id.text);
            }
        }

//
        private sealed class ItemAdapter : RecyclerView.Adapter
        {
            public override int ItemCount
            {
                get;
            }

            public ItemAdapter(int itemCount)
            {
                ItemCount = itemCount;
            }

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                if (holder is ViewHolder textView)
                {
                    textView.TextView.Text = $"Item {position}";
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