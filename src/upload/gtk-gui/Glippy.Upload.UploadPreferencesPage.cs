
// This file has been generated by the GUI designer. Do not modify.
namespace Glippy.Upload
{
	internal partial class UploadPreferencesPage
	{
		private global::Gtk.VBox vbox9;
		private global::Gtk.Frame frame2;
		private global::Gtk.Alignment GtkAlignment;
		private global::Gtk.VBox vbox10;
		private global::Gtk.HBox hbox7;
		private global::Gtk.VBox vbox12;
		private global::Gtk.Label labelPastebinUsername;
		private global::Gtk.Label labelPastebinPassword;
		private global::Gtk.VBox vbox11;
		private global::Gtk.Entry pastebinUsername;
		private global::Gtk.Entry pastebinPassword;
		private global::Gtk.HBox hbox1;
		private global::Gtk.Alignment alignment6;
		private global::Gtk.Alignment alignment5;
		private global::Gtk.Fixed fixed1;
		private global::Gtk.Button buttonPastebinLogin;
		private global::Gtk.Button buttonClear;
		private global::Gtk.Label labelPastebin;
		
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget Glippy.Upload.UploadPreferencesPage
			global::Stetic.BinContainer.Attach (this);
			this.Name = "Glippy.Upload.UploadPreferencesPage";
			// Container child Glippy.Upload.UploadPreferencesPage.Gtk.Container+ContainerChild
			this.vbox9 = new global::Gtk.VBox ();
			this.vbox9.Name = "vbox9";
			this.vbox9.Spacing = 8;
			this.vbox9.BorderWidth = ((uint)(12));
			// Container child vbox9.Gtk.Box+BoxChild
			this.frame2 = new global::Gtk.Frame ();
			this.frame2.Name = "frame2";
			this.frame2.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child frame2.Gtk.Container+ContainerChild
			this.GtkAlignment = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment.Name = "GtkAlignment";
			this.GtkAlignment.LeftPadding = ((uint)(12));
			// Container child GtkAlignment.Gtk.Container+ContainerChild
			this.vbox10 = new global::Gtk.VBox ();
			this.vbox10.Name = "vbox10";
			this.vbox10.BorderWidth = ((uint)(3));
			// Container child vbox10.Gtk.Box+BoxChild
			this.hbox7 = new global::Gtk.HBox ();
			this.hbox7.Name = "hbox7";
			this.hbox7.Spacing = 6;
			this.hbox7.BorderWidth = ((uint)(3));
			// Container child hbox7.Gtk.Box+BoxChild
			this.vbox12 = new global::Gtk.VBox ();
			this.vbox12.Name = "vbox12";
			this.vbox12.Spacing = 6;
			// Container child vbox12.Gtk.Box+BoxChild
			this.labelPastebinUsername = new global::Gtk.Label ();
			this.labelPastebinUsername.Name = "labelPastebinUsername";
			this.labelPastebinUsername.Xalign = 0F;
			this.labelPastebinUsername.LabelProp = global::Mono.Unix.Catalog.GetString ("_Username");
			this.labelPastebinUsername.UseUnderline = true;
			this.vbox12.Add (this.labelPastebinUsername);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.vbox12 [this.labelPastebinUsername]));
			w1.Position = 0;
			w1.Expand = false;
			w1.Fill = false;
			w1.Padding = ((uint)(5));
			// Container child vbox12.Gtk.Box+BoxChild
			this.labelPastebinPassword = new global::Gtk.Label ();
			this.labelPastebinPassword.Name = "labelPastebinPassword";
			this.labelPastebinPassword.Xalign = 0F;
			this.labelPastebinPassword.LabelProp = global::Mono.Unix.Catalog.GetString ("_Password");
			this.labelPastebinPassword.UseUnderline = true;
			this.vbox12.Add (this.labelPastebinPassword);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vbox12 [this.labelPastebinPassword]));
			w2.Position = 1;
			w2.Expand = false;
			w2.Fill = false;
			w2.Padding = ((uint)(5));
			this.hbox7.Add (this.vbox12);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox7 [this.vbox12]));
			w3.Position = 0;
			w3.Expand = false;
			w3.Fill = false;
			// Container child hbox7.Gtk.Box+BoxChild
			this.vbox11 = new global::Gtk.VBox ();
			this.vbox11.Name = "vbox11";
			this.vbox11.Spacing = 6;
			// Container child vbox11.Gtk.Box+BoxChild
			this.pastebinUsername = new global::Gtk.Entry ();
			this.pastebinUsername.CanFocus = true;
			this.pastebinUsername.Name = "pastebinUsername";
			this.pastebinUsername.IsEditable = true;
			this.pastebinUsername.InvisibleChar = '•';
			this.vbox11.Add (this.pastebinUsername);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vbox11 [this.pastebinUsername]));
			w4.Position = 0;
			w4.Expand = false;
			w4.Fill = false;
			// Container child vbox11.Gtk.Box+BoxChild
			this.pastebinPassword = new global::Gtk.Entry ();
			this.pastebinPassword.CanFocus = true;
			this.pastebinPassword.Name = "pastebinPassword";
			this.pastebinPassword.IsEditable = true;
			this.pastebinPassword.Visibility = false;
			this.pastebinPassword.InvisibleChar = '•';
			this.vbox11.Add (this.pastebinPassword);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.vbox11 [this.pastebinPassword]));
			w5.Position = 1;
			w5.Expand = false;
			w5.Fill = false;
			this.hbox7.Add (this.vbox11);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.hbox7 [this.vbox11]));
			w6.PackType = ((global::Gtk.PackType)(1));
			w6.Position = 1;
			this.vbox10.Add (this.hbox7);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vbox10 [this.hbox7]));
			w7.Position = 0;
			w7.Expand = false;
			w7.Fill = false;
			w7.Padding = ((uint)(5));
			// Container child vbox10.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Homogeneous = true;
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.alignment6 = new global::Gtk.Alignment (0.5F, 0.5F, 1F, 1F);
			this.alignment6.Name = "alignment6";
			this.hbox1.Add (this.alignment6);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.alignment6]));
			w8.Position = 0;
			// Container child hbox1.Gtk.Box+BoxChild
			this.alignment5 = new global::Gtk.Alignment (0.5F, 0.5F, 1F, 1F);
			this.alignment5.Name = "alignment5";
			// Container child alignment5.Gtk.Container+ContainerChild
			this.fixed1 = new global::Gtk.Fixed ();
			this.fixed1.HeightRequest = 26;
			this.fixed1.Name = "fixed1";
			this.fixed1.HasWindow = false;
			this.alignment5.Add (this.fixed1);
			this.hbox1.Add (this.alignment5);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.alignment5]));
			w10.Position = 1;
			// Container child hbox1.Gtk.Box+BoxChild
			this.buttonPastebinLogin = new global::Gtk.Button ();
			this.buttonPastebinLogin.CanDefault = true;
			this.buttonPastebinLogin.CanFocus = true;
			this.buttonPastebinLogin.Name = "buttonPastebinLogin";
			this.buttonPastebinLogin.UseUnderline = true;
			this.buttonPastebinLogin.Label = global::Mono.Unix.Catalog.GetString ("_Login");
			this.hbox1.Add (this.buttonPastebinLogin);
			global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.buttonPastebinLogin]));
			w11.Position = 2;
			// Container child hbox1.Gtk.Box+BoxChild
			this.buttonClear = new global::Gtk.Button ();
			this.buttonClear.Sensitive = false;
			this.buttonClear.CanDefault = true;
			this.buttonClear.CanFocus = true;
			this.buttonClear.Name = "buttonClear";
			this.buttonClear.UseUnderline = true;
			this.buttonClear.Label = global::Mono.Unix.Catalog.GetString ("Cl_ear");
			this.hbox1.Add (this.buttonClear);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.buttonClear]));
			w12.PackType = ((global::Gtk.PackType)(1));
			w12.Position = 3;
			this.vbox10.Add (this.hbox1);
			global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.vbox10 [this.hbox1]));
			w13.PackType = ((global::Gtk.PackType)(1));
			w13.Position = 1;
			w13.Expand = false;
			w13.Fill = false;
			this.GtkAlignment.Add (this.vbox10);
			this.frame2.Add (this.GtkAlignment);
			this.labelPastebin = new global::Gtk.Label ();
			this.labelPastebin.Name = "labelPastebin";
			this.labelPastebin.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Pastebin account</b>");
			this.labelPastebin.UseMarkup = true;
			this.frame2.LabelWidget = this.labelPastebin;
			this.vbox9.Add (this.frame2);
			global::Gtk.Box.BoxChild w16 = ((global::Gtk.Box.BoxChild)(this.vbox9 [this.frame2]));
			w16.Position = 0;
			w16.Expand = false;
			w16.Fill = false;
			this.Add (this.vbox9);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.labelPastebinUsername.MnemonicWidget = this.pastebinUsername;
			this.labelPastebinPassword.MnemonicWidget = this.pastebinPassword;
			this.Hide ();
			this.buttonPastebinLogin.Clicked += new global::System.EventHandler (this.OnButtonPastebinLoginClicked);
			this.buttonClear.Clicked += new global::System.EventHandler (this.OnButtonPastebinClearClicked);
		}
	}
}
