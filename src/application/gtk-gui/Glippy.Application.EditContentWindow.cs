
// This file has been generated by the GUI designer. Do not modify.
namespace Glippy.Application
{
	internal partial class EditContentWindow
	{
		private global::Gtk.VBox vbox2;
		private global::Gtk.HBox hbox2;
		private global::Gtk.VBox vbox3;
		private global::Gtk.Label labelContent;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TextView content;
		private global::Gtk.HBox hbox3;
		private global::Gtk.Alignment alignment1;
		private global::Gtk.Alignment alignment2;
		private global::Gtk.Fixed fixed1;
		private global::Gtk.Button buttonApply;
		private global::Gtk.Button buttonCancel;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget Glippy.Application.EditContentWindow
			this.Name = "Glippy.Application.EditContentWindow";
			this.Title = global::Mono.Unix.Catalog.GetString ("Edit clipboard content");
			this.WindowPosition = ((global::Gtk.WindowPosition)(1));
			this.BorderWidth = ((uint)(9));
			// Container child Glippy.Application.EditContentWindow.Gtk.Container+ContainerChild
			this.vbox2 = new global::Gtk.VBox ();
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			// Container child vbox2.Gtk.Box+BoxChild
			this.hbox2 = new global::Gtk.HBox ();
			this.hbox2.Name = "hbox2";
			this.hbox2.Spacing = 6;
			this.hbox2.BorderWidth = ((uint)(9));
			// Container child hbox2.Gtk.Box+BoxChild
			this.vbox3 = new global::Gtk.VBox ();
			this.vbox3.Name = "vbox3";
			this.vbox3.Spacing = 6;
			// Container child vbox3.Gtk.Box+BoxChild
			this.labelContent = new global::Gtk.Label ();
			this.labelContent.Name = "labelContent";
			this.labelContent.LabelProp = global::Mono.Unix.Catalog.GetString ("C_ontent");
			this.labelContent.UseUnderline = true;
			this.vbox3.Add (this.labelContent);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.labelContent]));
			w1.Position = 0;
			w1.Expand = false;
			w1.Fill = false;
			this.hbox2.Add (this.vbox3);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.vbox3]));
			w2.Position = 0;
			w2.Expand = false;
			w2.Fill = false;
			w2.Padding = ((uint)(5));
			// Container child hbox2.Gtk.Box+BoxChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.content = new global::Gtk.TextView ();
			this.content.CanDefault = true;
			this.content.CanFocus = true;
			this.content.Name = "content";
			this.content.WrapMode = ((global::Gtk.WrapMode)(2));
			this.GtkScrolledWindow.Add (this.content);
			this.hbox2.Add (this.GtkScrolledWindow);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.GtkScrolledWindow]));
			w4.Position = 1;
			this.vbox2.Add (this.hbox2);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.hbox2]));
			w5.Position = 0;
			// Container child vbox2.Gtk.Box+BoxChild
			this.hbox3 = new global::Gtk.HBox ();
			this.hbox3.Name = "hbox3";
			this.hbox3.Homogeneous = true;
			this.hbox3.Spacing = 6;
			// Container child hbox3.Gtk.Box+BoxChild
			this.alignment1 = new global::Gtk.Alignment (0.5F, 0.5F, 1F, 1F);
			this.hbox3.Add (this.alignment1);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.alignment1]));
			w6.Position = 0;
			// Container child hbox3.Gtk.Box+BoxChild
			this.alignment2 = new global::Gtk.Alignment (0.5F, 0.5F, 1F, 1F);
			this.alignment2.Name = "alignment2";
			// Container child alignment2.Gtk.Container+ContainerChild
			this.fixed1 = new global::Gtk.Fixed ();
			this.fixed1.HeightRequest = 26;
			this.fixed1.Name = "fixed1";
			this.fixed1.HasWindow = false;
			this.alignment2.Add (this.fixed1);
			this.hbox3.Add (this.alignment2);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.alignment2]));
			w8.Position = 1;
			// Container child hbox3.Gtk.Box+BoxChild
			this.buttonApply = new global::Gtk.Button ();
			this.buttonApply.CanDefault = true;
			this.buttonApply.CanFocus = true;
			this.buttonApply.Name = "buttonApply";
			this.buttonApply.UseUnderline = true;
			this.buttonApply.Label = global::Mono.Unix.Catalog.GetString ("_Apply");
			this.hbox3.Add (this.buttonApply);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.buttonApply]));
			w9.PackType = ((global::Gtk.PackType)(1));
			w9.Position = 2;
			// Container child hbox3.Gtk.Box+BoxChild
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.CanFocus = true;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = global::Mono.Unix.Catalog.GetString ("_Cancel");
			this.hbox3.Add (this.buttonCancel);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.buttonCancel]));
			w10.PackType = ((global::Gtk.PackType)(1));
			w10.Position = 3;
			this.vbox2.Add (this.hbox3);
			global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.hbox3]));
			w11.PackType = ((global::Gtk.PackType)(1));
			w11.Position = 1;
			w11.Expand = false;
			w11.Fill = false;
			this.Add (this.vbox2);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 370;
			this.DefaultHeight = 321;
			this.content.HasDefault = true;
			this.Show ();
			this.KeyPressEvent += new global::Gtk.KeyPressEventHandler (this.OnKeyPressEvent);
			this.buttonCancel.Clicked += new global::System.EventHandler (this.OnButtonCancelClicked);
			this.buttonApply.Clicked += new global::System.EventHandler (this.OnButtonApplyClicked);
		}
	}
}
