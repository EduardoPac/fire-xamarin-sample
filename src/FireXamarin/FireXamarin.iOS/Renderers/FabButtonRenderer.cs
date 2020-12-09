using System.ComponentModel;
using CoreGraphics;
using FireXamarin.Controls;
using FireXamarin.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(FabButton), typeof(FabButtonRenderer))]
namespace FireXamarin.iOS.Renderers
{
    public class FabButtonRenderer : FrameRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null) return;
            var element = e.NewElement as FabButton;

            Layer.ShadowRadius = element.ShadowRadius;
            Layer.ShadowColor = UIColor.LightGray.CGColor;
            Layer.ShadowOffset = new CGSize(2, 2);
            Layer.ShadowOpacity = element.ShadowOpacity;
            Layer.MasksToBounds = false;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if(e.PropertyName == View.IsVisibleProperty.PropertyName)
            {
                var element = sender as FabButton;

                Layer.ShadowRadius = element.ShadowRadius;
                Layer.ShadowColor = UIColor.LightGray.CGColor;
                Layer.ShadowOffset = new CGSize(2, 2);
                Layer.ShadowOpacity = element.ShadowOpacity;
                Layer.MasksToBounds = false;
            }
        }
    }
}