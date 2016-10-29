using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using Flobbster.Windows.Forms;

namespace PropertyBagTester
{
	public class PropBagTestForm : System.Windows.Forms.Form
	{
		private Flobbster.Windows.Forms.PropertyBag bag1;
		private Flobbster.Windows.Forms.PropertyBag bag2;
		private Flobbster.Windows.Forms.PropertyTable bag3;
		//
		private System.Windows.Forms.ListView listView;
		private System.Windows.Forms.PropertyGrid propertyGrid;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ContextMenu contextMenu;
		private System.Windows.Forms.MenuItem menuReset;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ImageList imageList;

		// A simple enumerated type used in the property bags.
		private enum Fruit
		{
			Apple, Banana, Orange, Peach, Pear
		}

		public PropBagTestForm()
		{
			InitializeComponent();

			// Create the first property bag and add some properties.
			bag1 = new PropertyBag();
			bag1.GetValue += new PropertySpecEventHandler(this.bag1_GetValue);
			bag1.SetValue += new PropertySpecEventHandler(this.bag1_SetValue);
			bag1.Properties.Add(new PropertySpec("Fruit", typeof(Fruit), null, null, Fruit.Orange));
			bag1.Properties.Add(new PropertySpec("Picture", typeof(Image), "Some Category",
				"This is a sample description."));
			ListViewItem item1 = new ListViewItem("Bag 1", 0);
			item1.Tag = bag1;
            listView.Items.Add(item1);

			// Create the second property bag and add some properties.
			bag2 = new PropertyBag();
			bag2.GetValue += new PropertySpecEventHandler(this.bag2_GetValue);
			bag2.SetValue += new PropertySpecEventHandler(this.bag2_SetValue);
			bag2.Properties.Add(new PropertySpec("Fruit", typeof(Fruit), null, null, Fruit.Banana));
			bag2.Properties.Add(new PropertySpec("Typeface", typeof(Font), "Another Category", null,
				new Font("Tahoma", 8.25f)));
			bag2.Properties.Add(new PropertySpec("Some Boolean", "System.Boolean", "Some Category", null,
				false));

			ListViewItem item2 = new ListViewItem("Bag 2", 0);
			item2.Tag = bag2;
			listView.Items.Add(item2);

			// This time, create a property table.  It uses a Hashtable to store
			// values, so we don't need to wire GetValue and SetValue events.
			bag3 = new PropertyTable();
			bag3.Properties.Add(new PropertySpec("Fruit", typeof(Fruit), null, null, Fruit.Orange));
			bag3.Properties.Add(new PropertySpec("Picture", typeof(Image), "Some Category",
				"This is a sample description."));
			bag3.Properties.Add(new PropertySpec("Typeface", typeof(Font), "Another Category", null,
				new Font("Tahoma", 8.25f)));
			bag3.Properties.Add(new PropertySpec("Some Boolean", "System.Boolean", "Some Category", null,
				false));
			bag3.Properties.Add(new PropertySpec("Number", "System.Int64", null, "A big number.", 1234567890L));

			// Create a property that uses additional attributes.
			PropertySpec ps = new PropertySpec("Can't Touch This", typeof(string), null,
				"This property is read-only.", "Some Default String");
			ps.Attributes = new Attribute[] {
												ReadOnlyAttribute.Yes
											};
			bag3.Properties.Add(ps);

			// Assign values to the properties above.
			bag3["Fruit"] = Fruit.Apple;
			bag3["Picture"] = null;
			bag3["Typeface"] = new Font("Times New Roman", 12f);
			bag3["Some Boolean"] = true;
			bag3["Number"] = 1234567890L;
			bag3["Can't Touch This"] = "Some Default String";

			ListViewItem item3 = new ListViewItem("Table", 0);
			item3.Tag = bag3;
			listView.Items.Add(item3);
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
				if(components != null) 
					components.Dispose();

			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(PropBagTestForm));
			this.listView = new System.Windows.Forms.ListView();
			this.propertyGrid = new System.Windows.Forms.PropertyGrid();
			this.contextMenu = new System.Windows.Forms.ContextMenu();
			this.menuReset = new System.Windows.Forms.MenuItem();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// listView
			// 
			this.listView.HideSelection = false;
			this.listView.LargeImageList = this.imageList;
			this.listView.Location = new System.Drawing.Point(8, 32);
			this.listView.Name = "listView";
			this.listView.Size = new System.Drawing.Size(168, 264);
			this.listView.TabIndex = 0;
			this.listView.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
			// 
			// propertyGrid
			// 
			this.propertyGrid.CommandsVisibleIfAvailable = true;
			this.propertyGrid.ContextMenu = this.contextMenu;
			this.propertyGrid.LargeButtons = false;
			this.propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.propertyGrid.Location = new System.Drawing.Point(184, 32);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.Size = new System.Drawing.Size(272, 264);
			this.propertyGrid.TabIndex = 1;
			this.propertyGrid.Text = "propertyGrid1";
			this.propertyGrid.ViewBackColor = System.Drawing.SystemColors.Window;
			this.propertyGrid.ViewForeColor = System.Drawing.SystemColors.WindowText;
			// 
			// contextMenu
			// 
			this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						this.menuReset});
			// 
			// menuReset
			// 
			this.menuReset.Index = 0;
			this.menuReset.Text = "Reset";
			this.menuReset.Click += new System.EventHandler(this.menuReset_Click);
			// 
			// imageList
			// 
			this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageList.ImageSize = new System.Drawing.Size(32, 32);
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(448, 23);
			this.label1.TabIndex = 2;
			this.label1.Text = "Select one or multiple items in the list to see those objects in the property gri" +
				"d.";
			// 
			// PropBagTestForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(464, 301);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.label1,
																		  this.propertyGrid,
																		  this.listView});
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PropBagTestForm";
			this.Text = "PropertyBag Test Form";
			this.ResumeLayout(false);

		}
		#endregion

		[STAThread] static void Main() { Application.Run(new PropBagTestForm()); }

		private void listView_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			ArrayList objs = new ArrayList();
			foreach(ListViewItem item in listView.SelectedItems)
				objs.Add(item.Tag);

			propertyGrid.SelectedObjects = objs.ToArray();
		}

		private void menuReset_Click(object sender, System.EventArgs e)
		{
			if(propertyGrid.SelectedObject != null &&
				propertyGrid.SelectedGridItem != null)
			{
				propertyGrid.ResetSelectedProperty();
			}
		}

		// Member variables associated with the properties of bag 1 and
		// bag 2.  Since events are fired to query these, you could use
		// any source--variables, contents of a file, a database, etc.
		private Fruit bag1_Fruit = Fruit.Orange;
		private Image bag1_Picture = null;

		private Fruit bag2_Fruit = Fruit.Banana;
		private Font bag2_Typeface = new Font("Tahoma", 8.25f);
		private bool bag2_SomeBoolean = false;

		// This is a pretty basic way to handle the properties.  Optimally,
		// you might have some kind of table that indexes into a database
		// or file where the values are stored.  But for the purposes of this
		// example, a simple case statement will do.
		private void bag1_GetValue(object sender, PropertySpecEventArgs e)
		{
			switch(e.Property.Name)
			{
				case "Fruit":  e.Value = bag1_Fruit;  break;
				case "Picture":  e.Value = bag1_Picture;  break;
			}
		}

		private void bag1_SetValue(object sender, PropertySpecEventArgs e)
		{
			switch(e.Property.Name)
			{
				case "Fruit":  bag1_Fruit = (Fruit)e.Value;  break;
				case "Picture":  bag1_Picture = (Image)e.Value;  break;
			}
		}

		private void bag2_GetValue(object sender, PropertySpecEventArgs e)
		{
			switch(e.Property.Name)
			{
				case "Fruit":  e.Value = bag2_Fruit;  break;
				case "Typeface":  e.Value = bag2_Typeface;  break;
				case "Some Boolean":  e.Value = bag2_SomeBoolean;  break;
			}
		}

		private void bag2_SetValue(object sender, PropertySpecEventArgs e)
		{
			switch(e.Property.Name)
			{
				case "Fruit":  bag2_Fruit = (Fruit)e.Value;  break;
				case "Typeface":  bag2_Typeface = (Font)e.Value;  break;
				case "Some Boolean":  bag2_SomeBoolean = (bool)e.Value;  break;
			}
		}
	}
}
