using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Reactive.Linq;


namespace Reactive.Bindings.Extensions
{
#pragma warning disable 1591
#pragma warning disable 0618
    public static class ViewEventExtensions
    {
        public static IObservable<Android.InputMethodServices.KeyboardView.KeyEventArgs> KeyAsObservable(this Android.InputMethodServices.KeyboardView self)
        {
            return Observable.FromEvent<EventHandler<Android.InputMethodServices.KeyboardView.KeyEventArgs>, Android.InputMethodServices.KeyboardView.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.Key += h,
                h => self.Key -= h);
        }
        public static IObservable<Android.InputMethodServices.KeyboardView.PressEventArgs> PressAsObservable(this Android.InputMethodServices.KeyboardView self)
        {
            return Observable.FromEvent<EventHandler<Android.InputMethodServices.KeyboardView.PressEventArgs>, Android.InputMethodServices.KeyboardView.PressEventArgs>(
                h => (s, e) => h(e),
                h => self.Press += h,
                h => self.Press -= h);
        }
        public static IObservable<Android.InputMethodServices.KeyboardView.ReleaseEventArgs> ReleaseAsObservable(this Android.InputMethodServices.KeyboardView self)
        {
            return Observable.FromEvent<EventHandler<Android.InputMethodServices.KeyboardView.ReleaseEventArgs>, Android.InputMethodServices.KeyboardView.ReleaseEventArgs>(
                h => (s, e) => h(e),
                h => self.Release += h,
                h => self.Release -= h);
        }
        public static IObservable<Android.InputMethodServices.KeyboardView.TextEventArgs> TextAsObservable(this Android.InputMethodServices.KeyboardView self)
        {
            return Observable.FromEvent<EventHandler<Android.InputMethodServices.KeyboardView.TextEventArgs>, Android.InputMethodServices.KeyboardView.TextEventArgs>(
                h => (s, e) => h(e),
                h => self.Text += h,
                h => self.Text -= h);
        }
        public static IObservable<EventArgs> SwipeDownEventAsObservable(this Android.InputMethodServices.KeyboardView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.SwipeDownEvent += h,
                h => self.SwipeDownEvent -= h);
        }
        public static IObservable<EventArgs> SwipeLeftEventAsObservable(this Android.InputMethodServices.KeyboardView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.SwipeLeftEvent += h,
                h => self.SwipeLeftEvent -= h);
        }
        public static IObservable<EventArgs> SwipeRightEventAsObservable(this Android.InputMethodServices.KeyboardView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.SwipeRightEvent += h,
                h => self.SwipeRightEvent -= h);
        }
        public static IObservable<EventArgs> SwipeUpEventAsObservable(this Android.InputMethodServices.KeyboardView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.SwipeUpEvent += h,
                h => self.SwipeUpEvent -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.InputMethodServices.KeyboardView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.InputMethodServices.KeyboardView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.InputMethodServices.KeyboardView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.InputMethodServices.KeyboardView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.InputMethodServices.KeyboardView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.InputMethodServices.KeyboardView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.InputMethodServices.KeyboardView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.InputMethodServices.KeyboardView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.InputMethodServices.KeyboardView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.InputMethodServices.KeyboardView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.InputMethodServices.KeyboardView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.InputMethodServices.KeyboardView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.InputMethodServices.KeyboardView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Opengl.GLSurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Opengl.GLSurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Opengl.GLSurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Opengl.GLSurfaceView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Opengl.GLSurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Opengl.GLSurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Opengl.GLSurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Opengl.GLSurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Opengl.GLSurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Opengl.GLSurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Opengl.GLSurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Opengl.GLSurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Opengl.GLSurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Views.View self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Views.View self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Views.View self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Views.View self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Views.View self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Views.View self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Views.View self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Views.View self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Views.View self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Views.View self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Views.View self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Views.View self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Views.View self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Widget.AbsListView.ScrollEventArgs> ScrollAsObservable(this Android.Widget.AbsListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AbsListView.ScrollEventArgs>, Android.Widget.AbsListView.ScrollEventArgs>(
                h => (s, e) => h(e),
                h => self.Scroll += h,
                h => self.Scroll -= h);
        }
        public static IObservable<Android.Widget.AbsListView.ScrollStateChangedEventArgs> ScrollStateChangedAsObservable(this Android.Widget.AbsListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AbsListView.ScrollStateChangedEventArgs>, Android.Widget.AbsListView.ScrollStateChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.ScrollStateChanged += h,
                h => self.ScrollStateChanged -= h);
        }
        public static IObservable<Android.Widget.AbsListView.RecyclerEventArgs> RecyclerAsObservable(this Android.Widget.AbsListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AbsListView.RecyclerEventArgs>, Android.Widget.AbsListView.RecyclerEventArgs>(
                h => (s, e) => h(e),
                h => self.Recycler += h,
                h => self.Recycler -= h);
        }
        public static IObservable<EventArgs> ItemSelectionClearedAsObservable(this Android.Widget.AbsListView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.ItemSelectionCleared += h,
                h => self.ItemSelectionCleared -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemClickEventArgs> ItemClickAsObservable(this Android.Widget.AbsListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemClickEventArgs>, Android.Widget.AdapterView.ItemClickEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemClick += h,
                h => self.ItemClick -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemLongClickEventArgs> ItemLongClickAsObservable(this Android.Widget.AbsListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemLongClickEventArgs>, Android.Widget.AdapterView.ItemLongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemLongClick += h,
                h => self.ItemLongClick -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemSelectedEventArgs> ItemSelectedAsObservable(this Android.Widget.AbsListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemSelectedEventArgs>, Android.Widget.AdapterView.ItemSelectedEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemSelected += h,
                h => self.ItemSelected -= h);
        }
        public static IObservable<Android.Widget.AdapterView.NothingSelectedEventArgs> NothingSelectedAsObservable(this Android.Widget.AbsListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.NothingSelectedEventArgs>, Android.Widget.AdapterView.NothingSelectedEventArgs>(
                h => (s, e) => h(e),
                h => self.NothingSelected += h,
                h => self.NothingSelected -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.AbsListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.AbsListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.AbsListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.AbsListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.AbsListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.AbsListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.AbsListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.AbsListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.AbsListView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.AbsListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.AbsListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.AbsListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.AbsListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.AbsListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.AbsListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.AbsListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.AbsListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.AbsListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<EventArgs> ItemSelectionClearedAsObservable(this Android.Widget.AdapterView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.ItemSelectionCleared += h,
                h => self.ItemSelectionCleared -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemClickEventArgs> ItemClickAsObservable(this Android.Widget.AdapterView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemClickEventArgs>, Android.Widget.AdapterView.ItemClickEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemClick += h,
                h => self.ItemClick -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemLongClickEventArgs> ItemLongClickAsObservable(this Android.Widget.AdapterView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemLongClickEventArgs>, Android.Widget.AdapterView.ItemLongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemLongClick += h,
                h => self.ItemLongClick -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemSelectedEventArgs> ItemSelectedAsObservable(this Android.Widget.AdapterView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemSelectedEventArgs>, Android.Widget.AdapterView.ItemSelectedEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemSelected += h,
                h => self.ItemSelected -= h);
        }
        public static IObservable<Android.Widget.AdapterView.NothingSelectedEventArgs> NothingSelectedAsObservable(this Android.Widget.AdapterView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.NothingSelectedEventArgs>, Android.Widget.AdapterView.NothingSelectedEventArgs>(
                h => (s, e) => h(e),
                h => self.NothingSelected += h,
                h => self.NothingSelected -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.AdapterView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.AdapterView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.AdapterView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.AdapterView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.AdapterView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.AdapterView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.AdapterView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.AdapterView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.AdapterView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.AdapterView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.AdapterView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.AdapterView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.AdapterView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.AdapterView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.AdapterView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.AdapterView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.AdapterView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.AdapterView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<EventArgs> ItemSelectionClearedAsObservable(this Android.Widget.AdapterViewAnimator self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.ItemSelectionCleared += h,
                h => self.ItemSelectionCleared -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemClickEventArgs> ItemClickAsObservable(this Android.Widget.AdapterViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemClickEventArgs>, Android.Widget.AdapterView.ItemClickEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemClick += h,
                h => self.ItemClick -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemLongClickEventArgs> ItemLongClickAsObservable(this Android.Widget.AdapterViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemLongClickEventArgs>, Android.Widget.AdapterView.ItemLongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemLongClick += h,
                h => self.ItemLongClick -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemSelectedEventArgs> ItemSelectedAsObservable(this Android.Widget.AdapterViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemSelectedEventArgs>, Android.Widget.AdapterView.ItemSelectedEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemSelected += h,
                h => self.ItemSelected -= h);
        }
        public static IObservable<Android.Widget.AdapterView.NothingSelectedEventArgs> NothingSelectedAsObservable(this Android.Widget.AdapterViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.NothingSelectedEventArgs>, Android.Widget.AdapterView.NothingSelectedEventArgs>(
                h => (s, e) => h(e),
                h => self.NothingSelected += h,
                h => self.NothingSelected -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.AdapterViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.AdapterViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.AdapterViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.AdapterViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.AdapterViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.AdapterViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.AdapterViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.AdapterViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.AdapterViewAnimator self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.AdapterViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.AdapterViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.AdapterViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.AdapterViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.AdapterViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.AdapterViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.AdapterViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.AdapterViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.AdapterViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<EventArgs> ItemSelectionClearedAsObservable(this Android.Widget.AutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.ItemSelectionCleared += h,
                h => self.ItemSelectionCleared -= h);
        }
        public static IObservable<EventArgs> DismissAsObservable(this Android.Widget.AutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Dismiss += h,
                h => self.Dismiss -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemClickEventArgs> ItemClickAsObservable(this Android.Widget.AutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemClickEventArgs>, Android.Widget.AdapterView.ItemClickEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemClick += h,
                h => self.ItemClick -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemSelectedEventArgs> ItemSelectedAsObservable(this Android.Widget.AutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemSelectedEventArgs>, Android.Widget.AdapterView.ItemSelectedEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemSelected += h,
                h => self.ItemSelected -= h);
        }
        public static IObservable<Android.Widget.AdapterView.NothingSelectedEventArgs> NothingSelectedAsObservable(this Android.Widget.AutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.NothingSelectedEventArgs>, Android.Widget.AdapterView.NothingSelectedEventArgs>(
                h => (s, e) => h(e),
                h => self.NothingSelected += h,
                h => self.NothingSelected -= h);
        }
        public static IObservable<Android.Text.AfterTextChangedEventArgs> AfterTextChangedAsObservable(this Android.Widget.AutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.AfterTextChangedEventArgs>, Android.Text.AfterTextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.AfterTextChanged += h,
                h => self.AfterTextChanged -= h);
        }
        public static IObservable<Android.Text.TextChangedEventArgs> BeforeTextChangedAsObservable(this Android.Widget.AutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.TextChangedEventArgs>, Android.Text.TextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.BeforeTextChanged += h,
                h => self.BeforeTextChanged -= h);
        }
        public static IObservable<Android.Text.TextChangedEventArgs> TextChangedAsObservable(this Android.Widget.AutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.TextChangedEventArgs>, Android.Text.TextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.TextChanged += h,
                h => self.TextChanged -= h);
        }
        public static IObservable<Android.Widget.TextView.EditorActionEventArgs> EditorActionAsObservable(this Android.Widget.AutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.TextView.EditorActionEventArgs>, Android.Widget.TextView.EditorActionEventArgs>(
                h => (s, e) => h(e),
                h => self.EditorAction += h,
                h => self.EditorAction -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.AutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.AutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.AutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.AutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.AutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.AutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.AutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.AutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.AutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.AutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.AutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.AutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.AutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.DatePicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.DatePicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.DatePicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.DatePicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.DatePicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.DatePicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.DatePicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.DatePicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.DatePicker self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.DatePicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.DatePicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.DatePicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.DatePicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.DatePicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.DatePicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.DatePicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.DatePicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.DatePicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<EventArgs> NextClickAsObservable(this Android.Widget.MediaController self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.NextClick += h,
                h => self.NextClick -= h);
        }
        public static IObservable<EventArgs> PreviousClickAsObservable(this Android.Widget.MediaController self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.PreviousClick += h,
                h => self.PreviousClick -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.MediaController self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.MediaController self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.MediaController self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.MediaController self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.MediaController self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.MediaController self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.MediaController self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.MediaController self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.MediaController self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.MediaController self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.MediaController self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.MediaController self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.MediaController self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.MediaController self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.MediaController self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.MediaController self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.MediaController self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.MediaController self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Text.AfterTextChangedEventArgs> AfterTextChangedAsObservable(this Android.Widget.TextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.AfterTextChangedEventArgs>, Android.Text.AfterTextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.AfterTextChanged += h,
                h => self.AfterTextChanged -= h);
        }
        public static IObservable<Android.Text.TextChangedEventArgs> BeforeTextChangedAsObservable(this Android.Widget.TextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.TextChangedEventArgs>, Android.Text.TextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.BeforeTextChanged += h,
                h => self.BeforeTextChanged -= h);
        }
        public static IObservable<Android.Text.TextChangedEventArgs> TextChangedAsObservable(this Android.Widget.TextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.TextChangedEventArgs>, Android.Text.TextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.TextChanged += h,
                h => self.TextChanged -= h);
        }
        public static IObservable<Android.Widget.TextView.EditorActionEventArgs> EditorActionAsObservable(this Android.Widget.TextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.TextView.EditorActionEventArgs>, Android.Widget.TextView.EditorActionEventArgs>(
                h => (s, e) => h(e),
                h => self.EditorAction += h,
                h => self.EditorAction -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.TextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.TextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.TextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.TextView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.TextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.TextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.TextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.TextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.TextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.TextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.TextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.TextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.TextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.App.FragmentBreadCrumbs.BreadCrumbClickEventArgs> BreadCrumbClickAsObservable(this Android.App.FragmentBreadCrumbs self)
        {
            return Observable.FromEvent<EventHandler<Android.App.FragmentBreadCrumbs.BreadCrumbClickEventArgs>, Android.App.FragmentBreadCrumbs.BreadCrumbClickEventArgs>(
                h => (s, e) => h(e),
                h => self.BreadCrumbClick += h,
                h => self.BreadCrumbClick -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.App.FragmentBreadCrumbs self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.App.FragmentBreadCrumbs self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.App.FragmentBreadCrumbs self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.App.FragmentBreadCrumbs self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.App.FragmentBreadCrumbs self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.App.FragmentBreadCrumbs self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.App.FragmentBreadCrumbs self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.App.FragmentBreadCrumbs self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.App.FragmentBreadCrumbs self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.App.FragmentBreadCrumbs self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.App.FragmentBreadCrumbs self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.App.FragmentBreadCrumbs self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.App.FragmentBreadCrumbs self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.App.FragmentBreadCrumbs self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.App.FragmentBreadCrumbs self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.App.FragmentBreadCrumbs self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.App.FragmentBreadCrumbs self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.App.FragmentBreadCrumbs self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<EventArgs> ExtendedSettingsClickAsObservable(this Android.App.MediaRouteButton self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.ExtendedSettingsClick += h,
                h => self.ExtendedSettingsClick -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.App.MediaRouteButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.App.MediaRouteButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.App.MediaRouteButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.App.MediaRouteButton self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.App.MediaRouteButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.App.MediaRouteButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.App.MediaRouteButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.App.MediaRouteButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.App.MediaRouteButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.App.MediaRouteButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.App.MediaRouteButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.App.MediaRouteButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.App.MediaRouteButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Appwidget.AppWidgetHostView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Appwidget.AppWidgetHostView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Appwidget.AppWidgetHostView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Appwidget.AppWidgetHostView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Appwidget.AppWidgetHostView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Appwidget.AppWidgetHostView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Appwidget.AppWidgetHostView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Appwidget.AppWidgetHostView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Appwidget.AppWidgetHostView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Appwidget.AppWidgetHostView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Appwidget.AppWidgetHostView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Appwidget.AppWidgetHostView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Appwidget.AppWidgetHostView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Appwidget.AppWidgetHostView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Appwidget.AppWidgetHostView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Appwidget.AppWidgetHostView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Appwidget.AppWidgetHostView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Appwidget.AppWidgetHostView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Gestures.GestureOverlayView.GestureEventArgs> GestureEventAsObservable(this Android.Gestures.GestureOverlayView self)
        {
            return Observable.FromEvent<EventHandler<Android.Gestures.GestureOverlayView.GestureEventArgs>, Android.Gestures.GestureOverlayView.GestureEventArgs>(
                h => (s, e) => h(e),
                h => self.GestureEvent += h,
                h => self.GestureEvent -= h);
        }
        public static IObservable<Android.Gestures.GestureOverlayView.GestureCancelledEventArgs> GestureCancelledAsObservable(this Android.Gestures.GestureOverlayView self)
        {
            return Observable.FromEvent<EventHandler<Android.Gestures.GestureOverlayView.GestureCancelledEventArgs>, Android.Gestures.GestureOverlayView.GestureCancelledEventArgs>(
                h => (s, e) => h(e),
                h => self.GestureCancelled += h,
                h => self.GestureCancelled -= h);
        }
        public static IObservable<Android.Gestures.GestureOverlayView.GestureEndedEventArgs> GestureEndedAsObservable(this Android.Gestures.GestureOverlayView self)
        {
            return Observable.FromEvent<EventHandler<Android.Gestures.GestureOverlayView.GestureEndedEventArgs>, Android.Gestures.GestureOverlayView.GestureEndedEventArgs>(
                h => (s, e) => h(e),
                h => self.GestureEnded += h,
                h => self.GestureEnded -= h);
        }
        public static IObservable<Android.Gestures.GestureOverlayView.GestureStartedEventArgs> GestureStartedAsObservable(this Android.Gestures.GestureOverlayView self)
        {
            return Observable.FromEvent<EventHandler<Android.Gestures.GestureOverlayView.GestureStartedEventArgs>, Android.Gestures.GestureOverlayView.GestureStartedEventArgs>(
                h => (s, e) => h(e),
                h => self.GestureStarted += h,
                h => self.GestureStarted -= h);
        }
        public static IObservable<Android.Gestures.GestureOverlayView.GesturePerformedEventArgs> GesturePerformedAsObservable(this Android.Gestures.GestureOverlayView self)
        {
            return Observable.FromEvent<EventHandler<Android.Gestures.GestureOverlayView.GesturePerformedEventArgs>, Android.Gestures.GestureOverlayView.GesturePerformedEventArgs>(
                h => (s, e) => h(e),
                h => self.GesturePerformed += h,
                h => self.GesturePerformed -= h);
        }
        public static IObservable<Android.Gestures.GestureOverlayView.GesturingEndedEventArgs> GesturingEndedAsObservable(this Android.Gestures.GestureOverlayView self)
        {
            return Observable.FromEvent<EventHandler<Android.Gestures.GestureOverlayView.GesturingEndedEventArgs>, Android.Gestures.GestureOverlayView.GesturingEndedEventArgs>(
                h => (s, e) => h(e),
                h => self.GesturingEnded += h,
                h => self.GesturingEnded -= h);
        }
        public static IObservable<Android.Gestures.GestureOverlayView.GesturingStartedEventArgs> GesturingStartedAsObservable(this Android.Gestures.GestureOverlayView self)
        {
            return Observable.FromEvent<EventHandler<Android.Gestures.GestureOverlayView.GesturingStartedEventArgs>, Android.Gestures.GestureOverlayView.GesturingStartedEventArgs>(
                h => (s, e) => h(e),
                h => self.GesturingStarted += h,
                h => self.GesturingStarted -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Gestures.GestureOverlayView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Gestures.GestureOverlayView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Gestures.GestureOverlayView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Gestures.GestureOverlayView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Gestures.GestureOverlayView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Gestures.GestureOverlayView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Gestures.GestureOverlayView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Gestures.GestureOverlayView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Gestures.GestureOverlayView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Gestures.GestureOverlayView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Gestures.GestureOverlayView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Gestures.GestureOverlayView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Gestures.GestureOverlayView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Gestures.GestureOverlayView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Gestures.GestureOverlayView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Gestures.GestureOverlayView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Gestures.GestureOverlayView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Gestures.GestureOverlayView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Text.AfterTextChangedEventArgs> AfterTextChangedAsObservable(this Android.InputMethodServices.ExtractEditText self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.AfterTextChangedEventArgs>, Android.Text.AfterTextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.AfterTextChanged += h,
                h => self.AfterTextChanged -= h);
        }
        public static IObservable<Android.Text.TextChangedEventArgs> BeforeTextChangedAsObservable(this Android.InputMethodServices.ExtractEditText self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.TextChangedEventArgs>, Android.Text.TextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.BeforeTextChanged += h,
                h => self.BeforeTextChanged -= h);
        }
        public static IObservable<Android.Text.TextChangedEventArgs> TextChangedAsObservable(this Android.InputMethodServices.ExtractEditText self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.TextChangedEventArgs>, Android.Text.TextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.TextChanged += h,
                h => self.TextChanged -= h);
        }
        public static IObservable<Android.Widget.TextView.EditorActionEventArgs> EditorActionAsObservable(this Android.InputMethodServices.ExtractEditText self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.TextView.EditorActionEventArgs>, Android.Widget.TextView.EditorActionEventArgs>(
                h => (s, e) => h(e),
                h => self.EditorAction += h,
                h => self.EditorAction -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.InputMethodServices.ExtractEditText self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.InputMethodServices.ExtractEditText self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.InputMethodServices.ExtractEditText self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.InputMethodServices.ExtractEditText self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.InputMethodServices.ExtractEditText self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.InputMethodServices.ExtractEditText self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.InputMethodServices.ExtractEditText self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.InputMethodServices.ExtractEditText self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.InputMethodServices.ExtractEditText self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.InputMethodServices.ExtractEditText self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.InputMethodServices.ExtractEditText self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.InputMethodServices.ExtractEditText self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.InputMethodServices.ExtractEditText self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Media.TV.TvView.UnhandledInputEventEventArgs> UnhandledInputEventAsObservable(this Android.Media.TV.TvView self)
        {
            return Observable.FromEvent<EventHandler<Android.Media.TV.TvView.UnhandledInputEventEventArgs>, Android.Media.TV.TvView.UnhandledInputEventEventArgs>(
                h => (s, e) => h(e),
                h => self.UnhandledInputEvent += h,
                h => self.UnhandledInputEvent -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Media.TV.TvView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Media.TV.TvView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Media.TV.TvView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Media.TV.TvView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Media.TV.TvView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Media.TV.TvView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Media.TV.TvView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Media.TV.TvView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Media.TV.TvView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Media.TV.TvView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Media.TV.TvView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Media.TV.TvView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Media.TV.TvView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Media.TV.TvView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Media.TV.TvView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Media.TV.TvView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Media.TV.TvView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Media.TV.TvView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Renderscripts.RSSurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Renderscripts.RSSurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Renderscripts.RSSurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Renderscripts.RSSurfaceView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Renderscripts.RSSurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Renderscripts.RSSurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Renderscripts.RSSurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Renderscripts.RSSurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Renderscripts.RSSurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Renderscripts.RSSurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Renderscripts.RSSurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Renderscripts.RSSurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Renderscripts.RSSurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Views.TextureView.SurfaceTextureAvailableEventArgs> SurfaceTextureAvailableAsObservable(this Android.Renderscripts.RSTextureView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.TextureView.SurfaceTextureAvailableEventArgs>, Android.Views.TextureView.SurfaceTextureAvailableEventArgs>(
                h => (s, e) => h(e),
                h => self.SurfaceTextureAvailable += h,
                h => self.SurfaceTextureAvailable -= h);
        }
        public static IObservable<Android.Views.TextureView.SurfaceTextureDestroyedEventArgs> SurfaceTextureDestroyedAsObservable(this Android.Renderscripts.RSTextureView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.TextureView.SurfaceTextureDestroyedEventArgs>, Android.Views.TextureView.SurfaceTextureDestroyedEventArgs>(
                h => (s, e) => h(e),
                h => self.SurfaceTextureDestroyed += h,
                h => self.SurfaceTextureDestroyed -= h);
        }
        public static IObservable<Android.Views.TextureView.SurfaceTextureSizeChangedEventArgs> SurfaceTextureSizeChangedAsObservable(this Android.Renderscripts.RSTextureView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.TextureView.SurfaceTextureSizeChangedEventArgs>, Android.Views.TextureView.SurfaceTextureSizeChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.SurfaceTextureSizeChanged += h,
                h => self.SurfaceTextureSizeChanged -= h);
        }
        public static IObservable<Android.Views.TextureView.SurfaceTextureUpdatedEventArgs> SurfaceTextureUpdatedAsObservable(this Android.Renderscripts.RSTextureView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.TextureView.SurfaceTextureUpdatedEventArgs>, Android.Views.TextureView.SurfaceTextureUpdatedEventArgs>(
                h => (s, e) => h(e),
                h => self.SurfaceTextureUpdated += h,
                h => self.SurfaceTextureUpdated -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Renderscripts.RSTextureView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Renderscripts.RSTextureView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Renderscripts.RSTextureView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Renderscripts.RSTextureView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Renderscripts.RSTextureView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Renderscripts.RSTextureView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Renderscripts.RSTextureView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Renderscripts.RSTextureView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Renderscripts.RSTextureView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Renderscripts.RSTextureView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Renderscripts.RSTextureView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Renderscripts.RSTextureView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Renderscripts.RSTextureView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Views.SurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Views.SurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Views.SurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Views.SurfaceView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Views.SurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Views.SurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Views.SurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Views.SurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Views.SurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Views.SurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Views.SurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Views.SurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Views.SurfaceView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Views.TextureView.SurfaceTextureAvailableEventArgs> SurfaceTextureAvailableAsObservable(this Android.Views.TextureView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.TextureView.SurfaceTextureAvailableEventArgs>, Android.Views.TextureView.SurfaceTextureAvailableEventArgs>(
                h => (s, e) => h(e),
                h => self.SurfaceTextureAvailable += h,
                h => self.SurfaceTextureAvailable -= h);
        }
        public static IObservable<Android.Views.TextureView.SurfaceTextureDestroyedEventArgs> SurfaceTextureDestroyedAsObservable(this Android.Views.TextureView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.TextureView.SurfaceTextureDestroyedEventArgs>, Android.Views.TextureView.SurfaceTextureDestroyedEventArgs>(
                h => (s, e) => h(e),
                h => self.SurfaceTextureDestroyed += h,
                h => self.SurfaceTextureDestroyed -= h);
        }
        public static IObservable<Android.Views.TextureView.SurfaceTextureSizeChangedEventArgs> SurfaceTextureSizeChangedAsObservable(this Android.Views.TextureView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.TextureView.SurfaceTextureSizeChangedEventArgs>, Android.Views.TextureView.SurfaceTextureSizeChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.SurfaceTextureSizeChanged += h,
                h => self.SurfaceTextureSizeChanged -= h);
        }
        public static IObservable<Android.Views.TextureView.SurfaceTextureUpdatedEventArgs> SurfaceTextureUpdatedAsObservable(this Android.Views.TextureView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.TextureView.SurfaceTextureUpdatedEventArgs>, Android.Views.TextureView.SurfaceTextureUpdatedEventArgs>(
                h => (s, e) => h(e),
                h => self.SurfaceTextureUpdated += h,
                h => self.SurfaceTextureUpdated -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Views.TextureView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Views.TextureView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Views.TextureView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Views.TextureView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Views.TextureView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Views.TextureView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Views.TextureView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Views.TextureView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Views.TextureView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Views.TextureView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Views.TextureView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Views.TextureView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Views.TextureView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Views.ViewGroup self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Views.ViewGroup self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Views.ViewGroup self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Views.ViewGroup self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Views.ViewGroup self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Views.ViewGroup self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Views.ViewGroup self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Views.ViewGroup self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Views.ViewGroup self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Views.ViewGroup self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Views.ViewGroup self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Views.ViewGroup self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Views.ViewGroup self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Views.ViewGroup self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Views.ViewGroup self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Views.ViewGroup self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Views.ViewGroup self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Views.ViewGroup self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Views.ViewStub.InflateEventArgs> InflateEventAsObservable(this Android.Views.ViewStub self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewStub.InflateEventArgs>, Android.Views.ViewStub.InflateEventArgs>(
                h => (s, e) => h(e),
                h => self.InflateEvent += h,
                h => self.InflateEvent -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Views.ViewStub self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Views.ViewStub self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Views.ViewStub self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Views.ViewStub self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Views.ViewStub self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Views.ViewStub self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Views.ViewStub self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Views.ViewStub self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Views.ViewStub self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Views.ViewStub self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Views.ViewStub self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Views.ViewStub self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Views.ViewStub self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Webkit.DownloadEventArgs> DownloadAsObservable(this Android.Webkit.WebView self)
        {
            return Observable.FromEvent<EventHandler<Android.Webkit.DownloadEventArgs>, Android.Webkit.DownloadEventArgs>(
                h => (s, e) => h(e),
                h => self.Download += h,
                h => self.Download -= h);
        }
        public static IObservable<Android.Webkit.WebView.FindEventArgs> FindAsObservable(this Android.Webkit.WebView self)
        {
            return Observable.FromEvent<EventHandler<Android.Webkit.WebView.FindEventArgs>, Android.Webkit.WebView.FindEventArgs>(
                h => (s, e) => h(e),
                h => self.Find += h,
                h => self.Find -= h);
        }
        public static IObservable<Android.Webkit.WebView.PictureEventArgs> PictureAsObservable(this Android.Webkit.WebView self)
        {
            return Observable.FromEvent<EventHandler<Android.Webkit.WebView.PictureEventArgs>, Android.Webkit.WebView.PictureEventArgs>(
                h => (s, e) => h(e),
                h => self.Picture += h,
                h => self.Picture -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Webkit.WebView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Webkit.WebView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Webkit.WebView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Webkit.WebView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Webkit.WebView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Webkit.WebView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Webkit.WebView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Webkit.WebView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Webkit.WebView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Webkit.WebView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Webkit.WebView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Webkit.WebView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Webkit.WebView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Webkit.WebView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Webkit.WebView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Webkit.WebView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Webkit.WebView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Webkit.WebView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.AbsSeekBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.AbsSeekBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.AbsSeekBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.AbsSeekBar self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.AbsSeekBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.AbsSeekBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.AbsSeekBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.AbsSeekBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.AbsSeekBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.AbsSeekBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.AbsSeekBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.AbsSeekBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.AbsSeekBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<EventArgs> ItemSelectionClearedAsObservable(this Android.Widget.AbsSpinner self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.ItemSelectionCleared += h,
                h => self.ItemSelectionCleared -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemClickEventArgs> ItemClickAsObservable(this Android.Widget.AbsSpinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemClickEventArgs>, Android.Widget.AdapterView.ItemClickEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemClick += h,
                h => self.ItemClick -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemLongClickEventArgs> ItemLongClickAsObservable(this Android.Widget.AbsSpinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemLongClickEventArgs>, Android.Widget.AdapterView.ItemLongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemLongClick += h,
                h => self.ItemLongClick -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemSelectedEventArgs> ItemSelectedAsObservable(this Android.Widget.AbsSpinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemSelectedEventArgs>, Android.Widget.AdapterView.ItemSelectedEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemSelected += h,
                h => self.ItemSelected -= h);
        }
        public static IObservable<Android.Widget.AdapterView.NothingSelectedEventArgs> NothingSelectedAsObservable(this Android.Widget.AbsSpinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.NothingSelectedEventArgs>, Android.Widget.AdapterView.NothingSelectedEventArgs>(
                h => (s, e) => h(e),
                h => self.NothingSelected += h,
                h => self.NothingSelected -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.AbsSpinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.AbsSpinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.AbsSpinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.AbsSpinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.AbsSpinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.AbsSpinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.AbsSpinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.AbsSpinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.AbsSpinner self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.AbsSpinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.AbsSpinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.AbsSpinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.AbsSpinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.AbsSpinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.AbsSpinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.AbsSpinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.AbsSpinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.AbsSpinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.AbsoluteLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.AbsoluteLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.AbsoluteLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.AbsoluteLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.AbsoluteLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.AbsoluteLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.AbsoluteLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.AbsoluteLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.AbsoluteLayout self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.AbsoluteLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.AbsoluteLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.AbsoluteLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.AbsoluteLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.AbsoluteLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.AbsoluteLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.AbsoluteLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.AbsoluteLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.AbsoluteLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Widget.ActionMenuView.MenuItemClickEventArgs> MenuItemClickAsObservable(this Android.Widget.ActionMenuView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.ActionMenuView.MenuItemClickEventArgs>, Android.Widget.ActionMenuView.MenuItemClickEventArgs>(
                h => (s, e) => h(e),
                h => self.MenuItemClick += h,
                h => self.MenuItemClick -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.ActionMenuView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.ActionMenuView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.ActionMenuView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.ActionMenuView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.ActionMenuView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.ActionMenuView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.ActionMenuView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.ActionMenuView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.ActionMenuView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.ActionMenuView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.ActionMenuView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.ActionMenuView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.ActionMenuView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.ActionMenuView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.ActionMenuView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.ActionMenuView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.ActionMenuView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.ActionMenuView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<EventArgs> ItemSelectionClearedAsObservable(this Android.Widget.AdapterViewFlipper self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.ItemSelectionCleared += h,
                h => self.ItemSelectionCleared -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemClickEventArgs> ItemClickAsObservable(this Android.Widget.AdapterViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemClickEventArgs>, Android.Widget.AdapterView.ItemClickEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemClick += h,
                h => self.ItemClick -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemLongClickEventArgs> ItemLongClickAsObservable(this Android.Widget.AdapterViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemLongClickEventArgs>, Android.Widget.AdapterView.ItemLongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemLongClick += h,
                h => self.ItemLongClick -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemSelectedEventArgs> ItemSelectedAsObservable(this Android.Widget.AdapterViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemSelectedEventArgs>, Android.Widget.AdapterView.ItemSelectedEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemSelected += h,
                h => self.ItemSelected -= h);
        }
        public static IObservable<Android.Widget.AdapterView.NothingSelectedEventArgs> NothingSelectedAsObservable(this Android.Widget.AdapterViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.NothingSelectedEventArgs>, Android.Widget.AdapterView.NothingSelectedEventArgs>(
                h => (s, e) => h(e),
                h => self.NothingSelected += h,
                h => self.NothingSelected -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.AdapterViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.AdapterViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.AdapterViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.AdapterViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.AdapterViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.AdapterViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.AdapterViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.AdapterViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.AdapterViewFlipper self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.AdapterViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.AdapterViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.AdapterViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.AdapterViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.AdapterViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.AdapterViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.AdapterViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.AdapterViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.AdapterViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.AnalogClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.AnalogClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.AnalogClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.AnalogClock self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.AnalogClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.AnalogClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.AnalogClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.AnalogClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.AnalogClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.AnalogClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.AnalogClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.AnalogClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.AnalogClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Text.AfterTextChangedEventArgs> AfterTextChangedAsObservable(this Android.Widget.Button self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.AfterTextChangedEventArgs>, Android.Text.AfterTextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.AfterTextChanged += h,
                h => self.AfterTextChanged -= h);
        }
        public static IObservable<Android.Text.TextChangedEventArgs> BeforeTextChangedAsObservable(this Android.Widget.Button self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.TextChangedEventArgs>, Android.Text.TextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.BeforeTextChanged += h,
                h => self.BeforeTextChanged -= h);
        }
        public static IObservable<Android.Text.TextChangedEventArgs> TextChangedAsObservable(this Android.Widget.Button self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.TextChangedEventArgs>, Android.Text.TextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.TextChanged += h,
                h => self.TextChanged -= h);
        }
        public static IObservable<Android.Widget.TextView.EditorActionEventArgs> EditorActionAsObservable(this Android.Widget.Button self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.TextView.EditorActionEventArgs>, Android.Widget.TextView.EditorActionEventArgs>(
                h => (s, e) => h(e),
                h => self.EditorAction += h,
                h => self.EditorAction -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.Button self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.Button self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.Button self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.Button self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.Button self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.Button self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.Button self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.Button self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.Button self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.Button self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.Button self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.Button self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.Button self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Widget.CalendarView.DateChangeEventArgs> DateChangeAsObservable(this Android.Widget.CalendarView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.CalendarView.DateChangeEventArgs>, Android.Widget.CalendarView.DateChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.DateChange += h,
                h => self.DateChange -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.CalendarView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.CalendarView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.CalendarView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.CalendarView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.CalendarView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.CalendarView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.CalendarView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.CalendarView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.CalendarView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.CalendarView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.CalendarView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.CalendarView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.CalendarView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.CalendarView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.CalendarView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.CalendarView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.CalendarView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.CalendarView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Widget.CompoundButton.CheckedChangeEventArgs> CheckedChangeAsObservable(this Android.Widget.CheckBox self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.CompoundButton.CheckedChangeEventArgs>, Android.Widget.CompoundButton.CheckedChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.CheckedChange += h,
                h => self.CheckedChange -= h);
        }
        public static IObservable<Android.Text.AfterTextChangedEventArgs> AfterTextChangedAsObservable(this Android.Widget.CheckBox self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.AfterTextChangedEventArgs>, Android.Text.AfterTextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.AfterTextChanged += h,
                h => self.AfterTextChanged -= h);
        }
        public static IObservable<Android.Text.TextChangedEventArgs> BeforeTextChangedAsObservable(this Android.Widget.CheckBox self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.TextChangedEventArgs>, Android.Text.TextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.BeforeTextChanged += h,
                h => self.BeforeTextChanged -= h);
        }
        public static IObservable<Android.Text.TextChangedEventArgs> TextChangedAsObservable(this Android.Widget.CheckBox self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.TextChangedEventArgs>, Android.Text.TextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.TextChanged += h,
                h => self.TextChanged -= h);
        }
        public static IObservable<Android.Widget.TextView.EditorActionEventArgs> EditorActionAsObservable(this Android.Widget.CheckBox self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.TextView.EditorActionEventArgs>, Android.Widget.TextView.EditorActionEventArgs>(
                h => (s, e) => h(e),
                h => self.EditorAction += h,
                h => self.EditorAction -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.CheckBox self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.CheckBox self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.CheckBox self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.CheckBox self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.CheckBox self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.CheckBox self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.CheckBox self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.CheckBox self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.CheckBox self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.CheckBox self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.CheckBox self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.CheckBox self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.CheckBox self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Text.AfterTextChangedEventArgs> AfterTextChangedAsObservable(this Android.Widget.CheckedTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.AfterTextChangedEventArgs>, Android.Text.AfterTextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.AfterTextChanged += h,
                h => self.AfterTextChanged -= h);
        }
        public static IObservable<Android.Text.TextChangedEventArgs> BeforeTextChangedAsObservable(this Android.Widget.CheckedTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.TextChangedEventArgs>, Android.Text.TextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.BeforeTextChanged += h,
                h => self.BeforeTextChanged -= h);
        }
        public static IObservable<Android.Text.TextChangedEventArgs> TextChangedAsObservable(this Android.Widget.CheckedTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.TextChangedEventArgs>, Android.Text.TextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.TextChanged += h,
                h => self.TextChanged -= h);
        }
        public static IObservable<Android.Widget.TextView.EditorActionEventArgs> EditorActionAsObservable(this Android.Widget.CheckedTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.TextView.EditorActionEventArgs>, Android.Widget.TextView.EditorActionEventArgs>(
                h => (s, e) => h(e),
                h => self.EditorAction += h,
                h => self.EditorAction -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.CheckedTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.CheckedTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.CheckedTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.CheckedTextView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.CheckedTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.CheckedTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.CheckedTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.CheckedTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.CheckedTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.CheckedTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.CheckedTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.CheckedTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.CheckedTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<EventArgs> ChronometerTickAsObservable(this Android.Widget.Chronometer self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.ChronometerTick += h,
                h => self.ChronometerTick -= h);
        }
        public static IObservable<Android.Text.AfterTextChangedEventArgs> AfterTextChangedAsObservable(this Android.Widget.Chronometer self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.AfterTextChangedEventArgs>, Android.Text.AfterTextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.AfterTextChanged += h,
                h => self.AfterTextChanged -= h);
        }
        public static IObservable<Android.Text.TextChangedEventArgs> BeforeTextChangedAsObservable(this Android.Widget.Chronometer self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.TextChangedEventArgs>, Android.Text.TextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.BeforeTextChanged += h,
                h => self.BeforeTextChanged -= h);
        }
        public static IObservable<Android.Text.TextChangedEventArgs> TextChangedAsObservable(this Android.Widget.Chronometer self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.TextChangedEventArgs>, Android.Text.TextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.TextChanged += h,
                h => self.TextChanged -= h);
        }
        public static IObservable<Android.Widget.TextView.EditorActionEventArgs> EditorActionAsObservable(this Android.Widget.Chronometer self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.TextView.EditorActionEventArgs>, Android.Widget.TextView.EditorActionEventArgs>(
                h => (s, e) => h(e),
                h => self.EditorAction += h,
                h => self.EditorAction -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.Chronometer self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.Chronometer self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.Chronometer self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.Chronometer self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.Chronometer self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.Chronometer self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.Chronometer self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.Chronometer self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.Chronometer self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.Chronometer self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.Chronometer self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.Chronometer self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.Chronometer self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Widget.CompoundButton.CheckedChangeEventArgs> CheckedChangeAsObservable(this Android.Widget.CompoundButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.CompoundButton.CheckedChangeEventArgs>, Android.Widget.CompoundButton.CheckedChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.CheckedChange += h,
                h => self.CheckedChange -= h);
        }
        public static IObservable<Android.Text.AfterTextChangedEventArgs> AfterTextChangedAsObservable(this Android.Widget.CompoundButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.AfterTextChangedEventArgs>, Android.Text.AfterTextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.AfterTextChanged += h,
                h => self.AfterTextChanged -= h);
        }
        public static IObservable<Android.Text.TextChangedEventArgs> BeforeTextChangedAsObservable(this Android.Widget.CompoundButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.TextChangedEventArgs>, Android.Text.TextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.BeforeTextChanged += h,
                h => self.BeforeTextChanged -= h);
        }
        public static IObservable<Android.Text.TextChangedEventArgs> TextChangedAsObservable(this Android.Widget.CompoundButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.TextChangedEventArgs>, Android.Text.TextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.TextChanged += h,
                h => self.TextChanged -= h);
        }
        public static IObservable<Android.Widget.TextView.EditorActionEventArgs> EditorActionAsObservable(this Android.Widget.CompoundButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.TextView.EditorActionEventArgs>, Android.Widget.TextView.EditorActionEventArgs>(
                h => (s, e) => h(e),
                h => self.EditorAction += h,
                h => self.EditorAction -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.CompoundButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.CompoundButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.CompoundButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.CompoundButton self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.CompoundButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.CompoundButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.CompoundButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.CompoundButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.CompoundButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.CompoundButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.CompoundButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.CompoundButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.CompoundButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.DialerFilter self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.DialerFilter self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.DialerFilter self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.DialerFilter self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.DialerFilter self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.DialerFilter self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.DialerFilter self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.DialerFilter self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.DialerFilter self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.DialerFilter self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.DialerFilter self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.DialerFilter self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.DialerFilter self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.DialerFilter self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.DialerFilter self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.DialerFilter self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.DialerFilter self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.DialerFilter self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Text.AfterTextChangedEventArgs> AfterTextChangedAsObservable(this Android.Widget.DigitalClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.AfterTextChangedEventArgs>, Android.Text.AfterTextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.AfterTextChanged += h,
                h => self.AfterTextChanged -= h);
        }
        public static IObservable<Android.Text.TextChangedEventArgs> BeforeTextChangedAsObservable(this Android.Widget.DigitalClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.TextChangedEventArgs>, Android.Text.TextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.BeforeTextChanged += h,
                h => self.BeforeTextChanged -= h);
        }
        public static IObservable<Android.Text.TextChangedEventArgs> TextChangedAsObservable(this Android.Widget.DigitalClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.TextChangedEventArgs>, Android.Text.TextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.TextChanged += h,
                h => self.TextChanged -= h);
        }
        public static IObservable<Android.Widget.TextView.EditorActionEventArgs> EditorActionAsObservable(this Android.Widget.DigitalClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.TextView.EditorActionEventArgs>, Android.Widget.TextView.EditorActionEventArgs>(
                h => (s, e) => h(e),
                h => self.EditorAction += h,
                h => self.EditorAction -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.DigitalClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.DigitalClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.DigitalClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.DigitalClock self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.DigitalClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.DigitalClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.DigitalClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.DigitalClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.DigitalClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.DigitalClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.DigitalClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.DigitalClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.DigitalClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Text.AfterTextChangedEventArgs> AfterTextChangedAsObservable(this Android.Widget.EditText self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.AfterTextChangedEventArgs>, Android.Text.AfterTextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.AfterTextChanged += h,
                h => self.AfterTextChanged -= h);
        }
        public static IObservable<Android.Text.TextChangedEventArgs> BeforeTextChangedAsObservable(this Android.Widget.EditText self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.TextChangedEventArgs>, Android.Text.TextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.BeforeTextChanged += h,
                h => self.BeforeTextChanged -= h);
        }
        public static IObservable<Android.Text.TextChangedEventArgs> TextChangedAsObservable(this Android.Widget.EditText self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.TextChangedEventArgs>, Android.Text.TextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.TextChanged += h,
                h => self.TextChanged -= h);
        }
        public static IObservable<Android.Widget.TextView.EditorActionEventArgs> EditorActionAsObservable(this Android.Widget.EditText self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.TextView.EditorActionEventArgs>, Android.Widget.TextView.EditorActionEventArgs>(
                h => (s, e) => h(e),
                h => self.EditorAction += h,
                h => self.EditorAction -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.EditText self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.EditText self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.EditText self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.EditText self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.EditText self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.EditText self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.EditText self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.EditText self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.EditText self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.EditText self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.EditText self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.EditText self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.EditText self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Widget.ExpandableListView.ChildClickEventArgs> ChildClickAsObservable(this Android.Widget.ExpandableListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.ExpandableListView.ChildClickEventArgs>, Android.Widget.ExpandableListView.ChildClickEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildClick += h,
                h => self.ChildClick -= h);
        }
        public static IObservable<Android.Widget.ExpandableListView.GroupClickEventArgs> GroupClickAsObservable(this Android.Widget.ExpandableListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.ExpandableListView.GroupClickEventArgs>, Android.Widget.ExpandableListView.GroupClickEventArgs>(
                h => (s, e) => h(e),
                h => self.GroupClick += h,
                h => self.GroupClick -= h);
        }
        public static IObservable<Android.Widget.ExpandableListView.GroupCollapseEventArgs> GroupCollapseAsObservable(this Android.Widget.ExpandableListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.ExpandableListView.GroupCollapseEventArgs>, Android.Widget.ExpandableListView.GroupCollapseEventArgs>(
                h => (s, e) => h(e),
                h => self.GroupCollapse += h,
                h => self.GroupCollapse -= h);
        }
        public static IObservable<Android.Widget.ExpandableListView.GroupExpandEventArgs> GroupExpandAsObservable(this Android.Widget.ExpandableListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.ExpandableListView.GroupExpandEventArgs>, Android.Widget.ExpandableListView.GroupExpandEventArgs>(
                h => (s, e) => h(e),
                h => self.GroupExpand += h,
                h => self.GroupExpand -= h);
        }
        public static IObservable<Android.Widget.AbsListView.ScrollEventArgs> ScrollAsObservable(this Android.Widget.ExpandableListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AbsListView.ScrollEventArgs>, Android.Widget.AbsListView.ScrollEventArgs>(
                h => (s, e) => h(e),
                h => self.Scroll += h,
                h => self.Scroll -= h);
        }
        public static IObservable<Android.Widget.AbsListView.ScrollStateChangedEventArgs> ScrollStateChangedAsObservable(this Android.Widget.ExpandableListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AbsListView.ScrollStateChangedEventArgs>, Android.Widget.AbsListView.ScrollStateChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.ScrollStateChanged += h,
                h => self.ScrollStateChanged -= h);
        }
        public static IObservable<Android.Widget.AbsListView.RecyclerEventArgs> RecyclerAsObservable(this Android.Widget.ExpandableListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AbsListView.RecyclerEventArgs>, Android.Widget.AbsListView.RecyclerEventArgs>(
                h => (s, e) => h(e),
                h => self.Recycler += h,
                h => self.Recycler -= h);
        }
        public static IObservable<EventArgs> ItemSelectionClearedAsObservable(this Android.Widget.ExpandableListView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.ItemSelectionCleared += h,
                h => self.ItemSelectionCleared -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemClickEventArgs> ItemClickAsObservable(this Android.Widget.ExpandableListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemClickEventArgs>, Android.Widget.AdapterView.ItemClickEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemClick += h,
                h => self.ItemClick -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemLongClickEventArgs> ItemLongClickAsObservable(this Android.Widget.ExpandableListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemLongClickEventArgs>, Android.Widget.AdapterView.ItemLongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemLongClick += h,
                h => self.ItemLongClick -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemSelectedEventArgs> ItemSelectedAsObservable(this Android.Widget.ExpandableListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemSelectedEventArgs>, Android.Widget.AdapterView.ItemSelectedEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemSelected += h,
                h => self.ItemSelected -= h);
        }
        public static IObservable<Android.Widget.AdapterView.NothingSelectedEventArgs> NothingSelectedAsObservable(this Android.Widget.ExpandableListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.NothingSelectedEventArgs>, Android.Widget.AdapterView.NothingSelectedEventArgs>(
                h => (s, e) => h(e),
                h => self.NothingSelected += h,
                h => self.NothingSelected -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.ExpandableListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.ExpandableListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.ExpandableListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.ExpandableListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.ExpandableListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.ExpandableListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.ExpandableListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.ExpandableListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.ExpandableListView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.ExpandableListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.ExpandableListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.ExpandableListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.ExpandableListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.ExpandableListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.ExpandableListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.ExpandableListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.ExpandableListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.ExpandableListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.FrameLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.FrameLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.FrameLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.FrameLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.FrameLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.FrameLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.FrameLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.FrameLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.FrameLayout self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.FrameLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.FrameLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.FrameLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.FrameLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.FrameLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.FrameLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.FrameLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.FrameLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.FrameLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<EventArgs> ItemSelectionClearedAsObservable(this Android.Widget.Gallery self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.ItemSelectionCleared += h,
                h => self.ItemSelectionCleared -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemClickEventArgs> ItemClickAsObservable(this Android.Widget.Gallery self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemClickEventArgs>, Android.Widget.AdapterView.ItemClickEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemClick += h,
                h => self.ItemClick -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemLongClickEventArgs> ItemLongClickAsObservable(this Android.Widget.Gallery self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemLongClickEventArgs>, Android.Widget.AdapterView.ItemLongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemLongClick += h,
                h => self.ItemLongClick -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemSelectedEventArgs> ItemSelectedAsObservable(this Android.Widget.Gallery self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemSelectedEventArgs>, Android.Widget.AdapterView.ItemSelectedEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemSelected += h,
                h => self.ItemSelected -= h);
        }
        public static IObservable<Android.Widget.AdapterView.NothingSelectedEventArgs> NothingSelectedAsObservable(this Android.Widget.Gallery self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.NothingSelectedEventArgs>, Android.Widget.AdapterView.NothingSelectedEventArgs>(
                h => (s, e) => h(e),
                h => self.NothingSelected += h,
                h => self.NothingSelected -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.Gallery self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.Gallery self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.Gallery self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.Gallery self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.Gallery self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.Gallery self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.Gallery self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.Gallery self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.Gallery self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.Gallery self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.Gallery self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.Gallery self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.Gallery self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.Gallery self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.Gallery self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.Gallery self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.Gallery self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.Gallery self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.GridLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.GridLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.GridLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.GridLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.GridLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.GridLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.GridLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.GridLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.GridLayout self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.GridLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.GridLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.GridLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.GridLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.GridLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.GridLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.GridLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.GridLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.GridLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Widget.AbsListView.ScrollEventArgs> ScrollAsObservable(this Android.Widget.GridView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AbsListView.ScrollEventArgs>, Android.Widget.AbsListView.ScrollEventArgs>(
                h => (s, e) => h(e),
                h => self.Scroll += h,
                h => self.Scroll -= h);
        }
        public static IObservable<Android.Widget.AbsListView.ScrollStateChangedEventArgs> ScrollStateChangedAsObservable(this Android.Widget.GridView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AbsListView.ScrollStateChangedEventArgs>, Android.Widget.AbsListView.ScrollStateChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.ScrollStateChanged += h,
                h => self.ScrollStateChanged -= h);
        }
        public static IObservable<Android.Widget.AbsListView.RecyclerEventArgs> RecyclerAsObservable(this Android.Widget.GridView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AbsListView.RecyclerEventArgs>, Android.Widget.AbsListView.RecyclerEventArgs>(
                h => (s, e) => h(e),
                h => self.Recycler += h,
                h => self.Recycler -= h);
        }
        public static IObservable<EventArgs> ItemSelectionClearedAsObservable(this Android.Widget.GridView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.ItemSelectionCleared += h,
                h => self.ItemSelectionCleared -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemClickEventArgs> ItemClickAsObservable(this Android.Widget.GridView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemClickEventArgs>, Android.Widget.AdapterView.ItemClickEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemClick += h,
                h => self.ItemClick -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemLongClickEventArgs> ItemLongClickAsObservable(this Android.Widget.GridView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemLongClickEventArgs>, Android.Widget.AdapterView.ItemLongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemLongClick += h,
                h => self.ItemLongClick -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemSelectedEventArgs> ItemSelectedAsObservable(this Android.Widget.GridView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemSelectedEventArgs>, Android.Widget.AdapterView.ItemSelectedEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemSelected += h,
                h => self.ItemSelected -= h);
        }
        public static IObservable<Android.Widget.AdapterView.NothingSelectedEventArgs> NothingSelectedAsObservable(this Android.Widget.GridView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.NothingSelectedEventArgs>, Android.Widget.AdapterView.NothingSelectedEventArgs>(
                h => (s, e) => h(e),
                h => self.NothingSelected += h,
                h => self.NothingSelected -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.GridView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.GridView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.GridView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.GridView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.GridView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.GridView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.GridView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.GridView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.GridView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.GridView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.GridView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.GridView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.GridView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.GridView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.GridView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.GridView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.GridView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.GridView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.HorizontalScrollView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.HorizontalScrollView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.HorizontalScrollView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.HorizontalScrollView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.HorizontalScrollView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.HorizontalScrollView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.HorizontalScrollView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.HorizontalScrollView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.HorizontalScrollView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.HorizontalScrollView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.HorizontalScrollView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.HorizontalScrollView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.HorizontalScrollView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.HorizontalScrollView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.HorizontalScrollView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.HorizontalScrollView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.HorizontalScrollView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.HorizontalScrollView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.ImageButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.ImageButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.ImageButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.ImageButton self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.ImageButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.ImageButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.ImageButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.ImageButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.ImageButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.ImageButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.ImageButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.ImageButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.ImageButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.ImageSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.ImageSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.ImageSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.ImageSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.ImageSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.ImageSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.ImageSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.ImageSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.ImageSwitcher self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.ImageSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.ImageSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.ImageSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.ImageSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.ImageSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.ImageSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.ImageSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.ImageSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.ImageSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.ImageView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.ImageView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.ImageView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.ImageView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.ImageView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.ImageView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.ImageView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.ImageView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.ImageView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.ImageView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.ImageView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.ImageView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.ImageView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.LinearLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.LinearLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.LinearLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.LinearLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.LinearLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.LinearLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.LinearLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.LinearLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.LinearLayout self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.LinearLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.LinearLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.LinearLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.LinearLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.LinearLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.LinearLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.LinearLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.LinearLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.LinearLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Widget.AbsListView.ScrollEventArgs> ScrollAsObservable(this Android.Widget.ListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AbsListView.ScrollEventArgs>, Android.Widget.AbsListView.ScrollEventArgs>(
                h => (s, e) => h(e),
                h => self.Scroll += h,
                h => self.Scroll -= h);
        }
        public static IObservable<Android.Widget.AbsListView.ScrollStateChangedEventArgs> ScrollStateChangedAsObservable(this Android.Widget.ListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AbsListView.ScrollStateChangedEventArgs>, Android.Widget.AbsListView.ScrollStateChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.ScrollStateChanged += h,
                h => self.ScrollStateChanged -= h);
        }
        public static IObservable<Android.Widget.AbsListView.RecyclerEventArgs> RecyclerAsObservable(this Android.Widget.ListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AbsListView.RecyclerEventArgs>, Android.Widget.AbsListView.RecyclerEventArgs>(
                h => (s, e) => h(e),
                h => self.Recycler += h,
                h => self.Recycler -= h);
        }
        public static IObservable<EventArgs> ItemSelectionClearedAsObservable(this Android.Widget.ListView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.ItemSelectionCleared += h,
                h => self.ItemSelectionCleared -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemClickEventArgs> ItemClickAsObservable(this Android.Widget.ListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemClickEventArgs>, Android.Widget.AdapterView.ItemClickEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemClick += h,
                h => self.ItemClick -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemLongClickEventArgs> ItemLongClickAsObservable(this Android.Widget.ListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemLongClickEventArgs>, Android.Widget.AdapterView.ItemLongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemLongClick += h,
                h => self.ItemLongClick -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemSelectedEventArgs> ItemSelectedAsObservable(this Android.Widget.ListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemSelectedEventArgs>, Android.Widget.AdapterView.ItemSelectedEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemSelected += h,
                h => self.ItemSelected -= h);
        }
        public static IObservable<Android.Widget.AdapterView.NothingSelectedEventArgs> NothingSelectedAsObservable(this Android.Widget.ListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.NothingSelectedEventArgs>, Android.Widget.AdapterView.NothingSelectedEventArgs>(
                h => (s, e) => h(e),
                h => self.NothingSelected += h,
                h => self.NothingSelected -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.ListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.ListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.ListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.ListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.ListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.ListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.ListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.ListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.ListView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.ListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.ListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.ListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.ListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.ListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.ListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.ListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.ListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.ListView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<EventArgs> ItemSelectionClearedAsObservable(this Android.Widget.MultiAutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.ItemSelectionCleared += h,
                h => self.ItemSelectionCleared -= h);
        }
        public static IObservable<EventArgs> DismissAsObservable(this Android.Widget.MultiAutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Dismiss += h,
                h => self.Dismiss -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemClickEventArgs> ItemClickAsObservable(this Android.Widget.MultiAutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemClickEventArgs>, Android.Widget.AdapterView.ItemClickEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemClick += h,
                h => self.ItemClick -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemSelectedEventArgs> ItemSelectedAsObservable(this Android.Widget.MultiAutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemSelectedEventArgs>, Android.Widget.AdapterView.ItemSelectedEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemSelected += h,
                h => self.ItemSelected -= h);
        }
        public static IObservable<Android.Widget.AdapterView.NothingSelectedEventArgs> NothingSelectedAsObservable(this Android.Widget.MultiAutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.NothingSelectedEventArgs>, Android.Widget.AdapterView.NothingSelectedEventArgs>(
                h => (s, e) => h(e),
                h => self.NothingSelected += h,
                h => self.NothingSelected -= h);
        }
        public static IObservable<Android.Text.AfterTextChangedEventArgs> AfterTextChangedAsObservable(this Android.Widget.MultiAutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.AfterTextChangedEventArgs>, Android.Text.AfterTextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.AfterTextChanged += h,
                h => self.AfterTextChanged -= h);
        }
        public static IObservable<Android.Text.TextChangedEventArgs> BeforeTextChangedAsObservable(this Android.Widget.MultiAutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.TextChangedEventArgs>, Android.Text.TextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.BeforeTextChanged += h,
                h => self.BeforeTextChanged -= h);
        }
        public static IObservable<Android.Text.TextChangedEventArgs> TextChangedAsObservable(this Android.Widget.MultiAutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.TextChangedEventArgs>, Android.Text.TextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.TextChanged += h,
                h => self.TextChanged -= h);
        }
        public static IObservable<Android.Widget.TextView.EditorActionEventArgs> EditorActionAsObservable(this Android.Widget.MultiAutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.TextView.EditorActionEventArgs>, Android.Widget.TextView.EditorActionEventArgs>(
                h => (s, e) => h(e),
                h => self.EditorAction += h,
                h => self.EditorAction -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.MultiAutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.MultiAutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.MultiAutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.MultiAutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.MultiAutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.MultiAutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.MultiAutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.MultiAutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.MultiAutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.MultiAutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.MultiAutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.MultiAutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.MultiAutoCompleteTextView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Widget.NumberPicker.ScrollEventArgs> ScrollAsObservable(this Android.Widget.NumberPicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.NumberPicker.ScrollEventArgs>, Android.Widget.NumberPicker.ScrollEventArgs>(
                h => (s, e) => h(e),
                h => self.Scroll += h,
                h => self.Scroll -= h);
        }
        public static IObservable<Android.Widget.NumberPicker.ValueChangeEventArgs> ValueChangedAsObservable(this Android.Widget.NumberPicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.NumberPicker.ValueChangeEventArgs>, Android.Widget.NumberPicker.ValueChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.ValueChanged += h,
                h => self.ValueChanged -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.NumberPicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.NumberPicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.NumberPicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.NumberPicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.NumberPicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.NumberPicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.NumberPicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.NumberPicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.NumberPicker self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.NumberPicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.NumberPicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.NumberPicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.NumberPicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.NumberPicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.NumberPicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.NumberPicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.NumberPicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.NumberPicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.ProgressBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.ProgressBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.ProgressBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.ProgressBar self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.ProgressBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.ProgressBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.ProgressBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.ProgressBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.ProgressBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.ProgressBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.ProgressBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.ProgressBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.ProgressBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.QuickContactBadge self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.QuickContactBadge self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.QuickContactBadge self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.QuickContactBadge self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.QuickContactBadge self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.QuickContactBadge self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.QuickContactBadge self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.QuickContactBadge self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.QuickContactBadge self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.QuickContactBadge self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.QuickContactBadge self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.QuickContactBadge self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.QuickContactBadge self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Widget.CompoundButton.CheckedChangeEventArgs> CheckedChangeAsObservable(this Android.Widget.RadioButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.CompoundButton.CheckedChangeEventArgs>, Android.Widget.CompoundButton.CheckedChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.CheckedChange += h,
                h => self.CheckedChange -= h);
        }
        public static IObservable<Android.Text.AfterTextChangedEventArgs> AfterTextChangedAsObservable(this Android.Widget.RadioButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.AfterTextChangedEventArgs>, Android.Text.AfterTextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.AfterTextChanged += h,
                h => self.AfterTextChanged -= h);
        }
        public static IObservable<Android.Text.TextChangedEventArgs> BeforeTextChangedAsObservable(this Android.Widget.RadioButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.TextChangedEventArgs>, Android.Text.TextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.BeforeTextChanged += h,
                h => self.BeforeTextChanged -= h);
        }
        public static IObservable<Android.Text.TextChangedEventArgs> TextChangedAsObservable(this Android.Widget.RadioButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.TextChangedEventArgs>, Android.Text.TextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.TextChanged += h,
                h => self.TextChanged -= h);
        }
        public static IObservable<Android.Widget.TextView.EditorActionEventArgs> EditorActionAsObservable(this Android.Widget.RadioButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.TextView.EditorActionEventArgs>, Android.Widget.TextView.EditorActionEventArgs>(
                h => (s, e) => h(e),
                h => self.EditorAction += h,
                h => self.EditorAction -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.RadioButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.RadioButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.RadioButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.RadioButton self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.RadioButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.RadioButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.RadioButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.RadioButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.RadioButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.RadioButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.RadioButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.RadioButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.RadioButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Widget.RadioGroup.CheckedChangeEventArgs> CheckedChangeAsObservable(this Android.Widget.RadioGroup self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.RadioGroup.CheckedChangeEventArgs>, Android.Widget.RadioGroup.CheckedChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.CheckedChange += h,
                h => self.CheckedChange -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.RadioGroup self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.RadioGroup self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.RadioGroup self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.RadioGroup self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.RadioGroup self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.RadioGroup self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.RadioGroup self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.RadioGroup self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.RadioGroup self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.RadioGroup self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.RadioGroup self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.RadioGroup self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.RadioGroup self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.RadioGroup self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.RadioGroup self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.RadioGroup self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.RadioGroup self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.RadioGroup self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Widget.RatingBar.RatingBarChangeEventArgs> RatingBarChangeAsObservable(this Android.Widget.RatingBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.RatingBar.RatingBarChangeEventArgs>, Android.Widget.RatingBar.RatingBarChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.RatingBarChange += h,
                h => self.RatingBarChange -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.RatingBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.RatingBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.RatingBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.RatingBar self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.RatingBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.RatingBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.RatingBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.RatingBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.RatingBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.RatingBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.RatingBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.RatingBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.RatingBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.RelativeLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.RelativeLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.RelativeLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.RelativeLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.RelativeLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.RelativeLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.RelativeLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.RelativeLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.RelativeLayout self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.RelativeLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.RelativeLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.RelativeLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.RelativeLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.RelativeLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.RelativeLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.RelativeLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.RelativeLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.RelativeLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.ScrollView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.ScrollView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.ScrollView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.ScrollView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.ScrollView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.ScrollView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.ScrollView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.ScrollView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.ScrollView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.ScrollView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.ScrollView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.ScrollView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.ScrollView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.ScrollView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.ScrollView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.ScrollView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.ScrollView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.ScrollView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Widget.SearchView.CloseEventArgs> CloseAsObservable(this Android.Widget.SearchView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.SearchView.CloseEventArgs>, Android.Widget.SearchView.CloseEventArgs>(
                h => (s, e) => h(e),
                h => self.Close += h,
                h => self.Close -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> QueryTextFocusChangeAsObservable(this Android.Widget.SearchView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.QueryTextFocusChange += h,
                h => self.QueryTextFocusChange -= h);
        }
        public static IObservable<Android.Widget.SearchView.QueryTextChangeEventArgs> QueryTextChangeAsObservable(this Android.Widget.SearchView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.SearchView.QueryTextChangeEventArgs>, Android.Widget.SearchView.QueryTextChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.QueryTextChange += h,
                h => self.QueryTextChange -= h);
        }
        public static IObservable<Android.Widget.SearchView.QueryTextSubmitEventArgs> QueryTextSubmitAsObservable(this Android.Widget.SearchView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.SearchView.QueryTextSubmitEventArgs>, Android.Widget.SearchView.QueryTextSubmitEventArgs>(
                h => (s, e) => h(e),
                h => self.QueryTextSubmit += h,
                h => self.QueryTextSubmit -= h);
        }
        public static IObservable<EventArgs> SearchClickAsObservable(this Android.Widget.SearchView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.SearchClick += h,
                h => self.SearchClick -= h);
        }
        public static IObservable<Android.Widget.SearchView.SuggestionClickEventArgs> SuggestionClickAsObservable(this Android.Widget.SearchView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.SearchView.SuggestionClickEventArgs>, Android.Widget.SearchView.SuggestionClickEventArgs>(
                h => (s, e) => h(e),
                h => self.SuggestionClick += h,
                h => self.SuggestionClick -= h);
        }
        public static IObservable<Android.Widget.SearchView.SuggestionSelectEventArgs> SuggestionSelectAsObservable(this Android.Widget.SearchView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.SearchView.SuggestionSelectEventArgs>, Android.Widget.SearchView.SuggestionSelectEventArgs>(
                h => (s, e) => h(e),
                h => self.SuggestionSelect += h,
                h => self.SuggestionSelect -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.SearchView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.SearchView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.SearchView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.SearchView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.SearchView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.SearchView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.SearchView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.SearchView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.SearchView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.SearchView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.SearchView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.SearchView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.SearchView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.SearchView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.SearchView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.SearchView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.SearchView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.SearchView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Widget.SeekBar.ProgressChangedEventArgs> ProgressChangedAsObservable(this Android.Widget.SeekBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.SeekBar.ProgressChangedEventArgs>, Android.Widget.SeekBar.ProgressChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.ProgressChanged += h,
                h => self.ProgressChanged -= h);
        }
        public static IObservable<Android.Widget.SeekBar.StartTrackingTouchEventArgs> StartTrackingTouchAsObservable(this Android.Widget.SeekBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.SeekBar.StartTrackingTouchEventArgs>, Android.Widget.SeekBar.StartTrackingTouchEventArgs>(
                h => (s, e) => h(e),
                h => self.StartTrackingTouch += h,
                h => self.StartTrackingTouch -= h);
        }
        public static IObservable<Android.Widget.SeekBar.StopTrackingTouchEventArgs> StopTrackingTouchAsObservable(this Android.Widget.SeekBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.SeekBar.StopTrackingTouchEventArgs>, Android.Widget.SeekBar.StopTrackingTouchEventArgs>(
                h => (s, e) => h(e),
                h => self.StopTrackingTouch += h,
                h => self.StopTrackingTouch -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.SeekBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.SeekBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.SeekBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.SeekBar self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.SeekBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.SeekBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.SeekBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.SeekBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.SeekBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.SeekBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.SeekBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.SeekBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.SeekBar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<EventArgs> DrawerCloseAsObservable(this Android.Widget.SlidingDrawer self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.DrawerClose += h,
                h => self.DrawerClose -= h);
        }
        public static IObservable<EventArgs> DrawerOpenAsObservable(this Android.Widget.SlidingDrawer self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.DrawerOpen += h,
                h => self.DrawerOpen -= h);
        }
        public static IObservable<EventArgs> ScrollEndedAsObservable(this Android.Widget.SlidingDrawer self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.ScrollEnded += h,
                h => self.ScrollEnded -= h);
        }
        public static IObservable<EventArgs> ScrollStartedAsObservable(this Android.Widget.SlidingDrawer self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.ScrollStarted += h,
                h => self.ScrollStarted -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.SlidingDrawer self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.SlidingDrawer self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.SlidingDrawer self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.SlidingDrawer self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.SlidingDrawer self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.SlidingDrawer self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.SlidingDrawer self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.SlidingDrawer self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.SlidingDrawer self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.SlidingDrawer self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.SlidingDrawer self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.SlidingDrawer self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.SlidingDrawer self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.SlidingDrawer self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.SlidingDrawer self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.SlidingDrawer self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.SlidingDrawer self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.SlidingDrawer self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.Space self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.Space self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.Space self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.Space self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.Space self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.Space self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.Space self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.Space self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.Space self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.Space self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.Space self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.Space self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.Space self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<EventArgs> ItemSelectionClearedAsObservable(this Android.Widget.Spinner self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.ItemSelectionCleared += h,
                h => self.ItemSelectionCleared -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemClickEventArgs> ItemClickAsObservable(this Android.Widget.Spinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemClickEventArgs>, Android.Widget.AdapterView.ItemClickEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemClick += h,
                h => self.ItemClick -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemLongClickEventArgs> ItemLongClickAsObservable(this Android.Widget.Spinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemLongClickEventArgs>, Android.Widget.AdapterView.ItemLongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemLongClick += h,
                h => self.ItemLongClick -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemSelectedEventArgs> ItemSelectedAsObservable(this Android.Widget.Spinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemSelectedEventArgs>, Android.Widget.AdapterView.ItemSelectedEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemSelected += h,
                h => self.ItemSelected -= h);
        }
        public static IObservable<Android.Widget.AdapterView.NothingSelectedEventArgs> NothingSelectedAsObservable(this Android.Widget.Spinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.NothingSelectedEventArgs>, Android.Widget.AdapterView.NothingSelectedEventArgs>(
                h => (s, e) => h(e),
                h => self.NothingSelected += h,
                h => self.NothingSelected -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.Spinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.Spinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.Spinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.Spinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.Spinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.Spinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.Spinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.Spinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.Spinner self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.Spinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.Spinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.Spinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.Spinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.Spinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.Spinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.Spinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.Spinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.Spinner self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<EventArgs> ItemSelectionClearedAsObservable(this Android.Widget.StackView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.ItemSelectionCleared += h,
                h => self.ItemSelectionCleared -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemClickEventArgs> ItemClickAsObservable(this Android.Widget.StackView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemClickEventArgs>, Android.Widget.AdapterView.ItemClickEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemClick += h,
                h => self.ItemClick -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemLongClickEventArgs> ItemLongClickAsObservable(this Android.Widget.StackView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemLongClickEventArgs>, Android.Widget.AdapterView.ItemLongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemLongClick += h,
                h => self.ItemLongClick -= h);
        }
        public static IObservable<Android.Widget.AdapterView.ItemSelectedEventArgs> ItemSelectedAsObservable(this Android.Widget.StackView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.ItemSelectedEventArgs>, Android.Widget.AdapterView.ItemSelectedEventArgs>(
                h => (s, e) => h(e),
                h => self.ItemSelected += h,
                h => self.ItemSelected -= h);
        }
        public static IObservable<Android.Widget.AdapterView.NothingSelectedEventArgs> NothingSelectedAsObservable(this Android.Widget.StackView self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.AdapterView.NothingSelectedEventArgs>, Android.Widget.AdapterView.NothingSelectedEventArgs>(
                h => (s, e) => h(e),
                h => self.NothingSelected += h,
                h => self.NothingSelected -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.StackView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.StackView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.StackView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.StackView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.StackView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.StackView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.StackView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.StackView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.StackView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.StackView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.StackView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.StackView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.StackView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.StackView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.StackView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.StackView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.StackView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.StackView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Widget.CompoundButton.CheckedChangeEventArgs> CheckedChangeAsObservable(this Android.Widget.Switch self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.CompoundButton.CheckedChangeEventArgs>, Android.Widget.CompoundButton.CheckedChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.CheckedChange += h,
                h => self.CheckedChange -= h);
        }
        public static IObservable<Android.Text.AfterTextChangedEventArgs> AfterTextChangedAsObservable(this Android.Widget.Switch self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.AfterTextChangedEventArgs>, Android.Text.AfterTextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.AfterTextChanged += h,
                h => self.AfterTextChanged -= h);
        }
        public static IObservable<Android.Text.TextChangedEventArgs> BeforeTextChangedAsObservable(this Android.Widget.Switch self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.TextChangedEventArgs>, Android.Text.TextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.BeforeTextChanged += h,
                h => self.BeforeTextChanged -= h);
        }
        public static IObservable<Android.Text.TextChangedEventArgs> TextChangedAsObservable(this Android.Widget.Switch self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.TextChangedEventArgs>, Android.Text.TextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.TextChanged += h,
                h => self.TextChanged -= h);
        }
        public static IObservable<Android.Widget.TextView.EditorActionEventArgs> EditorActionAsObservable(this Android.Widget.Switch self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.TextView.EditorActionEventArgs>, Android.Widget.TextView.EditorActionEventArgs>(
                h => (s, e) => h(e),
                h => self.EditorAction += h,
                h => self.EditorAction -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.Switch self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.Switch self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.Switch self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.Switch self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.Switch self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.Switch self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.Switch self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.Switch self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.Switch self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.Switch self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.Switch self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.Switch self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.Switch self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Widget.TabHost.TabChangeEventArgs> TabChangedAsObservable(this Android.Widget.TabHost self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.TabHost.TabChangeEventArgs>, Android.Widget.TabHost.TabChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.TabChanged += h,
                h => self.TabChanged -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.TabHost self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.TabHost self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.TabHost self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.TabHost self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.TabHost self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.TabHost self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.TabHost self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.TabHost self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.TabHost self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.TabHost self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.TabHost self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.TabHost self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.TabHost self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.TabHost self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.TabHost self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.TabHost self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.TabHost self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.TabHost self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.TabWidget self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.TabWidget self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.TabWidget self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.TabWidget self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.TabWidget self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.TabWidget self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.TabWidget self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.TabWidget self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.TabWidget self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.TabWidget self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.TabWidget self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.TabWidget self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.TabWidget self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.TabWidget self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.TabWidget self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.TabWidget self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.TabWidget self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.TabWidget self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.TableLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.TableLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.TableLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.TableLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.TableLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.TableLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.TableLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.TableLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.TableLayout self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.TableLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.TableLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.TableLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.TableLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.TableLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.TableLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.TableLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.TableLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.TableLayout self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.TableRow self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.TableRow self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.TableRow self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.TableRow self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.TableRow self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.TableRow self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.TableRow self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.TableRow self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.TableRow self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.TableRow self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.TableRow self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.TableRow self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.TableRow self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.TableRow self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.TableRow self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.TableRow self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.TableRow self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.TableRow self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Text.AfterTextChangedEventArgs> AfterTextChangedAsObservable(this Android.Widget.TextClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.AfterTextChangedEventArgs>, Android.Text.AfterTextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.AfterTextChanged += h,
                h => self.AfterTextChanged -= h);
        }
        public static IObservable<Android.Text.TextChangedEventArgs> BeforeTextChangedAsObservable(this Android.Widget.TextClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.TextChangedEventArgs>, Android.Text.TextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.BeforeTextChanged += h,
                h => self.BeforeTextChanged -= h);
        }
        public static IObservable<Android.Text.TextChangedEventArgs> TextChangedAsObservable(this Android.Widget.TextClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.TextChangedEventArgs>, Android.Text.TextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.TextChanged += h,
                h => self.TextChanged -= h);
        }
        public static IObservable<Android.Widget.TextView.EditorActionEventArgs> EditorActionAsObservable(this Android.Widget.TextClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.TextView.EditorActionEventArgs>, Android.Widget.TextView.EditorActionEventArgs>(
                h => (s, e) => h(e),
                h => self.EditorAction += h,
                h => self.EditorAction -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.TextClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.TextClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.TextClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.TextClock self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.TextClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.TextClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.TextClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.TextClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.TextClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.TextClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.TextClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.TextClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.TextClock self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.TextSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.TextSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.TextSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.TextSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.TextSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.TextSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.TextSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.TextSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.TextSwitcher self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.TextSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.TextSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.TextSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.TextSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.TextSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.TextSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.TextSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.TextSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.TextSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Widget.TimePicker.TimeChangedEventArgs> TimeChangedAsObservable(this Android.Widget.TimePicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.TimePicker.TimeChangedEventArgs>, Android.Widget.TimePicker.TimeChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.TimeChanged += h,
                h => self.TimeChanged -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.TimePicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.TimePicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.TimePicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.TimePicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.TimePicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.TimePicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.TimePicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.TimePicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.TimePicker self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.TimePicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.TimePicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.TimePicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.TimePicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.TimePicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.TimePicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.TimePicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.TimePicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.TimePicker self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Widget.CompoundButton.CheckedChangeEventArgs> CheckedChangeAsObservable(this Android.Widget.ToggleButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.CompoundButton.CheckedChangeEventArgs>, Android.Widget.CompoundButton.CheckedChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.CheckedChange += h,
                h => self.CheckedChange -= h);
        }
        public static IObservable<Android.Text.AfterTextChangedEventArgs> AfterTextChangedAsObservable(this Android.Widget.ToggleButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.AfterTextChangedEventArgs>, Android.Text.AfterTextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.AfterTextChanged += h,
                h => self.AfterTextChanged -= h);
        }
        public static IObservable<Android.Text.TextChangedEventArgs> BeforeTextChangedAsObservable(this Android.Widget.ToggleButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.TextChangedEventArgs>, Android.Text.TextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.BeforeTextChanged += h,
                h => self.BeforeTextChanged -= h);
        }
        public static IObservable<Android.Text.TextChangedEventArgs> TextChangedAsObservable(this Android.Widget.ToggleButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Text.TextChangedEventArgs>, Android.Text.TextChangedEventArgs>(
                h => (s, e) => h(e),
                h => self.TextChanged += h,
                h => self.TextChanged -= h);
        }
        public static IObservable<Android.Widget.TextView.EditorActionEventArgs> EditorActionAsObservable(this Android.Widget.ToggleButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.TextView.EditorActionEventArgs>, Android.Widget.TextView.EditorActionEventArgs>(
                h => (s, e) => h(e),
                h => self.EditorAction += h,
                h => self.EditorAction -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.ToggleButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.ToggleButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.ToggleButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.ToggleButton self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.ToggleButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.ToggleButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.ToggleButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.ToggleButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.ToggleButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.ToggleButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.ToggleButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.ToggleButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.ToggleButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<EventArgs> NavigationOnClickAsObservable(this Android.Widget.Toolbar self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.NavigationOnClick += h,
                h => self.NavigationOnClick -= h);
        }
        public static IObservable<Android.Widget.Toolbar.MenuItemClickEventArgs> MenuItemClickAsObservable(this Android.Widget.Toolbar self)
        {
            return Observable.FromEvent<EventHandler<Android.Widget.Toolbar.MenuItemClickEventArgs>, Android.Widget.Toolbar.MenuItemClickEventArgs>(
                h => (s, e) => h(e),
                h => self.MenuItemClick += h,
                h => self.MenuItemClick -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.Toolbar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.Toolbar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.Toolbar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.Toolbar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.Toolbar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.Toolbar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.Toolbar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.Toolbar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.Toolbar self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.Toolbar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.Toolbar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.Toolbar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.Toolbar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.Toolbar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.Toolbar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.Toolbar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.Toolbar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.Toolbar self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.TwoLineListItem self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.TwoLineListItem self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.TwoLineListItem self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.TwoLineListItem self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.TwoLineListItem self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.TwoLineListItem self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.TwoLineListItem self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.TwoLineListItem self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.TwoLineListItem self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.TwoLineListItem self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.TwoLineListItem self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.TwoLineListItem self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.TwoLineListItem self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.TwoLineListItem self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.TwoLineListItem self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.TwoLineListItem self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.TwoLineListItem self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.TwoLineListItem self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<EventArgs> CompletionAsObservable(this Android.Widget.VideoView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Completion += h,
                h => self.Completion -= h);
        }
        public static IObservable<Android.Media.MediaPlayer.ErrorEventArgs> ErrorAsObservable(this Android.Widget.VideoView self)
        {
            return Observable.FromEvent<EventHandler<Android.Media.MediaPlayer.ErrorEventArgs>, Android.Media.MediaPlayer.ErrorEventArgs>(
                h => (s, e) => h(e),
                h => self.Error += h,
                h => self.Error -= h);
        }
        public static IObservable<Android.Media.MediaPlayer.InfoEventArgs> InfoAsObservable(this Android.Widget.VideoView self)
        {
            return Observable.FromEvent<EventHandler<Android.Media.MediaPlayer.InfoEventArgs>, Android.Media.MediaPlayer.InfoEventArgs>(
                h => (s, e) => h(e),
                h => self.Info += h,
                h => self.Info -= h);
        }
        public static IObservable<EventArgs> PreparedAsObservable(this Android.Widget.VideoView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Prepared += h,
                h => self.Prepared -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.VideoView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.VideoView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.VideoView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.VideoView self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.VideoView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.VideoView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.VideoView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.VideoView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.VideoView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.VideoView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.VideoView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.VideoView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.VideoView self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.ViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.ViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.ViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.ViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.ViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.ViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.ViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.ViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.ViewAnimator self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.ViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.ViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.ViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.ViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.ViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.ViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.ViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.ViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.ViewAnimator self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.ViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.ViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.ViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.ViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.ViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.ViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.ViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.ViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.ViewFlipper self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.ViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.ViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.ViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.ViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.ViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.ViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.ViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.ViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.ViewFlipper self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.ViewSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.ViewSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.ViewSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.ViewSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.ViewSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.ViewSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.ViewSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.ViewSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.ViewSwitcher self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.ViewSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.ViewSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.ViewSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.ViewSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.ViewSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.ViewSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.ViewSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.ViewSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.ViewSwitcher self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.ZoomButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.ZoomButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.ZoomButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.ZoomButton self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.ZoomButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.ZoomButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.ZoomButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.ZoomButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.ZoomButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.ZoomButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.ZoomButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.ZoomButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.ZoomButton self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
        public static IObservable<EventArgs> ZoomInClickAsObservable(this Android.Widget.ZoomControls self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.ZoomInClick += h,
                h => self.ZoomInClick -= h);
        }
        public static IObservable<EventArgs> ZoomOutClickAsObservable(this Android.Widget.ZoomControls self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.ZoomOutClick += h,
                h => self.ZoomOutClick -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewAddedEventArgs> ChildViewAddedAsObservable(this Android.Widget.ZoomControls self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewAddedEventArgs>, Android.Views.ViewGroup.ChildViewAddedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewAdded += h,
                h => self.ChildViewAdded -= h);
        }
        public static IObservable<Android.Views.ViewGroup.ChildViewRemovedEventArgs> ChildViewRemovedAsObservable(this Android.Widget.ZoomControls self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.ViewGroup.ChildViewRemovedEventArgs>, Android.Views.ViewGroup.ChildViewRemovedEventArgs>(
                h => (s, e) => h(e),
                h => self.ChildViewRemoved += h,
                h => self.ChildViewRemoved -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationEndEventArgs> AnimationEndAsObservable(this Android.Widget.ZoomControls self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationEndEventArgs>, Android.Views.Animations.Animation.AnimationEndEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationEnd += h,
                h => self.AnimationEnd -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationRepeatEventArgs> AnimationRepeatAsObservable(this Android.Widget.ZoomControls self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationRepeatEventArgs>, Android.Views.Animations.Animation.AnimationRepeatEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationRepeat += h,
                h => self.AnimationRepeat -= h);
        }
        public static IObservable<Android.Views.Animations.Animation.AnimationStartEventArgs> AnimationStartAsObservable(this Android.Widget.ZoomControls self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.Animations.Animation.AnimationStartEventArgs>, Android.Views.Animations.Animation.AnimationStartEventArgs>(
                h => (s, e) => h(e),
                h => self.AnimationStart += h,
                h => self.AnimationStart -= h);
        }
        public static IObservable<Android.Views.View.ViewAttachedToWindowEventArgs> ViewAttachedToWindowAsObservable(this Android.Widget.ZoomControls self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewAttachedToWindowEventArgs>, Android.Views.View.ViewAttachedToWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewAttachedToWindow += h,
                h => self.ViewAttachedToWindow -= h);
        }
        public static IObservable<Android.Views.View.ViewDetachedFromWindowEventArgs> ViewDetachedFromWindowAsObservable(this Android.Widget.ZoomControls self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.ViewDetachedFromWindowEventArgs>, Android.Views.View.ViewDetachedFromWindowEventArgs>(
                h => (s, e) => h(e),
                h => self.ViewDetachedFromWindow += h,
                h => self.ViewDetachedFromWindow -= h);
        }
        public static IObservable<Android.Views.View.LayoutChangeEventArgs> LayoutChangeAsObservable(this Android.Widget.ZoomControls self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LayoutChangeEventArgs>, Android.Views.View.LayoutChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.LayoutChange += h,
                h => self.LayoutChange -= h);
        }
        public static IObservable<EventArgs> ClickAsObservable(this Android.Widget.ZoomControls self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Click += h,
                h => self.Click -= h);
        }
        public static IObservable<Android.Views.View.CreateContextMenuEventArgs> ContextMenuCreatedAsObservable(this Android.Widget.ZoomControls self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.CreateContextMenuEventArgs>, Android.Views.View.CreateContextMenuEventArgs>(
                h => (s, e) => h(e),
                h => self.ContextMenuCreated += h,
                h => self.ContextMenuCreated -= h);
        }
        public static IObservable<Android.Views.View.DragEventArgs> DragAsObservable(this Android.Widget.ZoomControls self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.DragEventArgs>, Android.Views.View.DragEventArgs>(
                h => (s, e) => h(e),
                h => self.Drag += h,
                h => self.Drag -= h);
        }
        public static IObservable<Android.Views.View.GenericMotionEventArgs> GenericMotionAsObservable(this Android.Widget.ZoomControls self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.GenericMotionEventArgs>, Android.Views.View.GenericMotionEventArgs>(
                h => (s, e) => h(e),
                h => self.GenericMotion += h,
                h => self.GenericMotion -= h);
        }
        public static IObservable<Android.Views.View.HoverEventArgs> HoverAsObservable(this Android.Widget.ZoomControls self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.HoverEventArgs>, Android.Views.View.HoverEventArgs>(
                h => (s, e) => h(e),
                h => self.Hover += h,
                h => self.Hover -= h);
        }
        public static IObservable<Android.Views.View.KeyEventArgs> KeyPressAsObservable(this Android.Widget.ZoomControls self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.KeyEventArgs>, Android.Views.View.KeyEventArgs>(
                h => (s, e) => h(e),
                h => self.KeyPress += h,
                h => self.KeyPress -= h);
        }
        public static IObservable<Android.Views.View.LongClickEventArgs> LongClickAsObservable(this Android.Widget.ZoomControls self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.LongClickEventArgs>, Android.Views.View.LongClickEventArgs>(
                h => (s, e) => h(e),
                h => self.LongClick += h,
                h => self.LongClick -= h);
        }
        public static IObservable<Android.Views.View.SystemUiVisibilityChangeEventArgs> SystemUiVisibilityChangeAsObservable(this Android.Widget.ZoomControls self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.SystemUiVisibilityChangeEventArgs>, Android.Views.View.SystemUiVisibilityChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.SystemUiVisibilityChange += h,
                h => self.SystemUiVisibilityChange -= h);
        }
        public static IObservable<Android.Views.View.TouchEventArgs> TouchAsObservable(this Android.Widget.ZoomControls self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.TouchEventArgs>, Android.Views.View.TouchEventArgs>(
                h => (s, e) => h(e),
                h => self.Touch += h,
                h => self.Touch -= h);
        }
        public static IObservable<Android.Views.View.FocusChangeEventArgs> FocusChangeAsObservable(this Android.Widget.ZoomControls self)
        {
            return Observable.FromEvent<EventHandler<Android.Views.View.FocusChangeEventArgs>, Android.Views.View.FocusChangeEventArgs>(
                h => (s, e) => h(e),
                h => self.FocusChange += h,
                h => self.FocusChange -= h);
        }
    }
#pragma warning restore 1591
#pragma warning restore 0618
}