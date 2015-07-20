
// This file has been generated by the GUI designer. Do not modify.
namespace Glippy.Actions
{
	internal partial class EditActionWindow
	{
		private global::Gtk.VBox dialog1_VBox;
		private global::Gtk.Alignment alignment3;
		private global::Gtk.VBox vbox1;
		private global::Gtk.Label label1;
		private global::Gtk.Label label2;
		private global::Gtk.HBox hbox1;
		private global::Gtk.VBox vbox2;
		private global::Gtk.Label labelLabel;
		private global::Gtk.Label labelContent;
		private global::Gtk.VBox vbox3;
		private global::Gtk.Entry label;
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
			// Widget Glippy.Actions.EditActionWindow
			this.Name = "Glippy.Actions.EditActionWindow";
			this.Title = global::Mono.Unix.Catalog.GetString ("Add action");
			this.WindowPosition = ((global::Gtk.WindowPosition)(1));
			this.BorderWidth = ((uint)(9));
			// Container child Glippy.Actions.EditActionWindow.Gtk.Container+ContainerChild
			this.dialog1_VBox = new global::Gtk.VBox ();
			this.dialog1_VBox.Name = "dialog1_VBox";
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.alignment3 = new global::Gtk.Alignment (0.5F, 0.5F, 1F, 1F);
			this.alignment3.Name = "alignment3";
			this.alignment3.BorderWidth = ((uint)(12));
			// Container child alignment3.Gtk.Container+ContainerChild
			this.vbox1 = new global::Gtk.VBox ();
			this.vbox1.Name = "vbox1";
			this.vbox1.Spacing = 6;
			// Container child vbox1.Gtk.Box+BoxChild
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.Xalign = 0F;
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("Both label and content are required.");
			this.vbox1.Add (this.label1);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.label1]));
			w1.Position = 0;
			w1.Expand = false;
			w1.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.label2 = new global::Gtk.Label ();
			this.label2.Name = "label2";
			this.label2.Xalign = 0F;
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("You can use underscore in label for menu shortcut and %s in content as text placeholder. Each line is executed separately.");
			this.label2.Wrap = true;
			this.vbox1.Add (this.label2);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.label2]));
			w2.Position = 1;
			w2.Expand = false;
			w2.Fill = false;
			this.alignment3.Add (this.vbox1);
			this.dialog1_VBox.Add (this.alignment3);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.dialog1_VBox [this.alignment3]));
			w4.Position = 0;
			w4.Expand = false;
			w4.Fill = false;
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			this.hbox1.BorderWidth = ((uint)(6));
			// Container child hbox1.Gtk.Box+BoxChild
			this.vbox2 = new global::Gtk.VBox ();
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			this.vbox2.BorderWidth = ((uint)(6));
			// Container child vbox2.Gtk.Box+BoxChild
			this.labelLabel = new global::Gtk.Label ();
			this.labelLabel.Name = "labelLabel";
			this.labelLabel.LabelProp = global::Mono.Unix.Catalog.GetString ("_Label");
			this.labelLabel.UseUnderline = true;
			this.vbox2.Add (this.labelLabel);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.labelLabel]));
			w5.Position = 0;
			w5.Expand = false;
			w5.Fill = false;
			w5.Padding = ((uint)(1));
			// Container child vbox2.Gtk.Box+BoxChild
			this.labelContent = new global::Gtk.Label ();
			this.labelContent.Name = "labelContent";
			this.labelContent.LabelProp = global::Mono.Unix.Catalog.GetString ("C_ontent");
			this.labelContent.UseUnderline = true;
			this.vbox2.Add (this.labelContent);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.labelContent]));
			w6.Position = 1;
			w6.Expand = false;
			w6.Fill = false;
			w6.Padding = ((uint)(4));
			this.hbox1.Add (this.vbox2);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.vbox2]));
			w7.Position = 0;
			w7.Expand = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.vbox3 = new global::Gtk.VBox ();
			this.vbox3.Name = "vbox3";
			this.vbox3.Spacing = 6;
			// Container child vbox3.Gtk.Box+BoxChild
			this.label = new global::Gtk.Entry ();
			this.label.CanDefault = true;
			this.label.CanFocus = true;
			this.label.Name = "label";
			this.label.IsEditable = true;
			this.label.InvisibleChar = '•';
			this.vbox3.Add (this.label);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.label]));
			w8.Position = 0;
			w8.Expand = false;
			w8.Fill = false;
			// Container child vbox3.Gtk.Box+BoxChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.content = new global::Gtk.TextView ();
			this.content.HeightRequest = 169;
			this.content.CanDefault = true;
			this.content.CanFocus = true;
			this.content.Name = "content";
			this.GtkScrolledWindow.Add (this.content);
			this.vbox3.Add (this.GtkScrolledWindow);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.GtkScrolledWindow]));
			w10.Position = 1;
			this.hbox1.Add (this.vbox3);
			global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.vbox3]));
			w11.Position = 1;
			this.dialog1_VBox.Add (this.hbox1);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.dialog1_VBox [this.hbox1]));
			w12.Position = 1;
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.hbox3 = new global::Gtk.HBox ();
			this.hbox3.Name = "hbox3";
			this.hbox3.Homogeneous = true;
			this.hbox3.Spacing = 6;
			// Container child hbox3.Gtk.Box+BoxChild
			this.alignment1 = new global::Gtk.Alignment (0.5F, 0.5F, 1F, 1F);
			this.alignment1.Name = "alignment1";
			this.hbox3.Add (this.alignment1);
			global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.alignment1]));
			w13.Position = 0;
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
			global::Gtk.Box.BoxChild w15 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.alignment2]));
			w15.Position = 1;
			// Container child hbox3.Gtk.Box+BoxChild
			this.buttonApply = new global::Gtk.Button ();
			this.buttonApply.CanDefault = true;
			this.buttonApply.CanFocus = true;
			this.buttonApply.Name = "buttonApply";
			this.buttonApply.UseUnderline = true;
			this.buttonApply.Label = global::Mono.Unix.Catalog.GetString ("_Apply");
			this.hbox3.Add (this.buttonApply);
			global::Gtk.Box.BoxChild w16 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.buttonApply]));
			w16.PackType = ((global::Gtk.PackType)(1));
			w16.Position = 2;
			// Container child hbox3.Gtk.Box+BoxChild
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.CanFocus = true;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = global::Mono.Unix.Catalog.GetString ("_Cancel");
			this.hbox3.Add (this.buttonCancel);
			global::Gtk.Box.BoxChild w17 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.buttonCancel]));
			w17.PackType = ((global::Gtk.PackType)(1));
			w17.Position = 3;
			this.dialog1_VBox.Add (this.hbox3);
			global::Gtk.Box.BoxChild w18 = ((global::Gtk.Box.BoxChild)(this.dialog1_VBox [this.hbox3]));
			w18.Position = 2;
			w18.Expand = false;
			w18.Fill = false;
			this.Add (this.dialog1_VBox);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 333;
			this.DefaultHeight = 361;
			this.label.HasDefault = true;
			this.Show ();
			this.KeyPressEvent += new global::Gtk.KeyPressEventHandler (this.OnKeyPressEvent);
			this.buttonCancel.Clicked += new global::System.EventHandler (this.OnButtonCancelClicked);
			this.buttonApply.Clicked += new global::System.EventHandler (this.OnButtonApplyClicked);
		}
	}
}
