namespace ReportGen
{
    partial class ExprsEdit
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExprsEdit));
            this.txtExpression = new System.Windows.Forms.TextBox();
            this.lblEditType = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOkay = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.insertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tagNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.globalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.todaysDateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.randomNumberToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.anyFieldNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.docCommandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteColumnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteParagraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shrinkRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.literalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.truthToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.falseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.operatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.subtractToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.multiplyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.divideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modulusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logicalORToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logicalANDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.functionWrapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.parenthesesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.absoluteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.roundingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.roundUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.roundDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.negativeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cosineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tangentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hyperbolicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sineHyperbolicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cosineHyperbolicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tangentHyperBolicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.arcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.arcSineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.arcCosineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.arcTangentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rootsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.naturalLogarithmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.base10LogarithmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exponentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.suffixToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.makeOrdinalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtLabel = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.lblEvaluatesTo = new System.Windows.Forms.Label();
            this.ticker = new System.Windows.Forms.Timer(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtExpression
            // 
            this.txtExpression.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExpression.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtExpression.Location = new System.Drawing.Point(12, 89);
            this.txtExpression.Multiline = true;
            this.txtExpression.Name = "txtExpression";
            this.txtExpression.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtExpression.Size = new System.Drawing.Size(441, 105);
            this.txtExpression.TabIndex = 6;
            this.txtExpression.Text = "1 == 1";
            this.txtExpression.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtExpression.TextChanged += new System.EventHandler(this.txtExpression_TextChanged);
            // 
            // lblEditType
            // 
            this.lblEditType.AutoSize = true;
            this.lblEditType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.lblEditType.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEditType.Location = new System.Drawing.Point(12, 34);
            this.lblEditType.Name = "lblEditType";
            this.lblEditType.Size = new System.Drawing.Size(122, 29);
            this.lblEditType.TabIndex = 14;
            this.lblEditType.Text = "Edit Type:";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(378, 336);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOkay
            // 
            this.btnOkay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOkay.Location = new System.Drawing.Point(12, 336);
            this.btnOkay.Name = "btnOkay";
            this.btnOkay.Size = new System.Drawing.Size(75, 23);
            this.btnOkay.TabIndex = 7;
            this.btnOkay.Text = "Accept";
            this.btnOkay.UseVisualStyleBackColor = true;
            this.btnOkay.Click += new System.EventHandler(this.btnOkay_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertToolStripMenuItem,
            this.functionWrapToolStripMenuItem,
            this.clearToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(470, 24);
            this.menuStrip1.TabIndex = 10;
            this.menuStrip1.TabStop = true;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // insertToolStripMenuItem
            // 
            this.insertToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tagNameToolStripMenuItem,
            this.literalToolStripMenuItem,
            this.operatorToolStripMenuItem});
            this.insertToolStripMenuItem.Name = "insertToolStripMenuItem";
            this.insertToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.insertToolStripMenuItem.Text = "Insert";
            // 
            // tagNameToolStripMenuItem
            // 
            this.tagNameToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.globalToolStripMenuItem,
            this.docCommandToolStripMenuItem});
            this.tagNameToolStripMenuItem.Name = "tagNameToolStripMenuItem";
            this.tagNameToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.tagNameToolStripMenuItem.Text = "<TagName>";
            // 
            // globalToolStripMenuItem
            // 
            this.globalToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.todaysDateToolStripMenuItem,
            this.randomNumberToolStripMenuItem,
            this.anyFieldNameToolStripMenuItem});
            this.globalToolStripMenuItem.Name = "globalToolStripMenuItem";
            this.globalToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.globalToolStripMenuItem.Text = "Globals Value";
            // 
            // todaysDateToolStripMenuItem
            // 
            this.todaysDateToolStripMenuItem.Name = "todaysDateToolStripMenuItem";
            this.todaysDateToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.todaysDateToolStripMenuItem.Text = "Today\'s Date";
            this.todaysDateToolStripMenuItem.Click += new System.EventHandler(this.todaysDateToolStripMenuItem_Click);
            // 
            // randomNumberToolStripMenuItem
            // 
            this.randomNumberToolStripMenuItem.Name = "randomNumberToolStripMenuItem";
            this.randomNumberToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.randomNumberToolStripMenuItem.Text = "Random Number";
            this.randomNumberToolStripMenuItem.Click += new System.EventHandler(this.randomNumberToolStripMenuItem_Click);
            // 
            // anyFieldNameToolStripMenuItem
            // 
            this.anyFieldNameToolStripMenuItem.Name = "anyFieldNameToolStripMenuItem";
            this.anyFieldNameToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.anyFieldNameToolStripMenuItem.Text = "Any Field Name";
            this.anyFieldNameToolStripMenuItem.Click += new System.EventHandler(this.anyFieldNameToolStripMenuItem_Click);
            // 
            // docCommandToolStripMenuItem
            // 
            this.docCommandToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteTableToolStripMenuItem,
            this.deleteColumnToolStripMenuItem,
            this.deleteRowToolStripMenuItem,
            this.deleteParagraphToolStripMenuItem,
            this.shrinkRowToolStripMenuItem});
            this.docCommandToolStripMenuItem.Name = "docCommandToolStripMenuItem";
            this.docCommandToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.docCommandToolStripMenuItem.Text = "Doc. Command";
            // 
            // deleteTableToolStripMenuItem
            // 
            this.deleteTableToolStripMenuItem.Name = "deleteTableToolStripMenuItem";
            this.deleteTableToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.deleteTableToolStripMenuItem.Text = "Delete Table";
            this.deleteTableToolStripMenuItem.Click += new System.EventHandler(this.deleteTableToolStripMenuItem_Click);
            // 
            // deleteColumnToolStripMenuItem
            // 
            this.deleteColumnToolStripMenuItem.Name = "deleteColumnToolStripMenuItem";
            this.deleteColumnToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.deleteColumnToolStripMenuItem.Text = "Delete Column";
            this.deleteColumnToolStripMenuItem.Click += new System.EventHandler(this.deleteColumnToolStripMenuItem_Click);
            // 
            // deleteRowToolStripMenuItem
            // 
            this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
            this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.deleteRowToolStripMenuItem.Text = "Delete Row";
            this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
            // 
            // deleteParagraphToolStripMenuItem
            // 
            this.deleteParagraphToolStripMenuItem.Name = "deleteParagraphToolStripMenuItem";
            this.deleteParagraphToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.deleteParagraphToolStripMenuItem.Text = "Delete Paragraph";
            this.deleteParagraphToolStripMenuItem.Click += new System.EventHandler(this.deleteParagraphToolStripMenuItem_Click);
            // 
            // shrinkRowToolStripMenuItem
            // 
            this.shrinkRowToolStripMenuItem.Name = "shrinkRowToolStripMenuItem";
            this.shrinkRowToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.shrinkRowToolStripMenuItem.Text = "Shrink Row";
            this.shrinkRowToolStripMenuItem.Click += new System.EventHandler(this.shrinkRowToolStripMenuItem_Click);
            // 
            // literalToolStripMenuItem
            // 
            this.literalToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.textToolStripMenuItem,
            this.textToolStripMenuItem1,
            this.truthToolStripMenuItem,
            this.dateToolStripMenuItem});
            this.literalToolStripMenuItem.Name = "literalToolStripMenuItem";
            this.literalToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.literalToolStripMenuItem.Text = "Value";
            // 
            // textToolStripMenuItem
            // 
            this.textToolStripMenuItem.Name = "textToolStripMenuItem";
            this.textToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.textToolStripMenuItem.Text = "Number";
            // 
            // textToolStripMenuItem1
            // 
            this.textToolStripMenuItem1.Name = "textToolStripMenuItem1";
            this.textToolStripMenuItem1.Size = new System.Drawing.Size(118, 22);
            this.textToolStripMenuItem1.Text = "Text";
            // 
            // truthToolStripMenuItem
            // 
            this.truthToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.trueToolStripMenuItem,
            this.falseToolStripMenuItem});
            this.truthToolStripMenuItem.Name = "truthToolStripMenuItem";
            this.truthToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.truthToolStripMenuItem.Text = "Truth";
            // 
            // trueToolStripMenuItem
            // 
            this.trueToolStripMenuItem.Name = "trueToolStripMenuItem";
            this.trueToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.trueToolStripMenuItem.Text = "True";
            // 
            // falseToolStripMenuItem
            // 
            this.falseToolStripMenuItem.Name = "falseToolStripMenuItem";
            this.falseToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.falseToolStripMenuItem.Text = "False";
            // 
            // dateToolStripMenuItem
            // 
            this.dateToolStripMenuItem.Name = "dateToolStripMenuItem";
            this.dateToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.dateToolStripMenuItem.Text = "Date";
            // 
            // operatorToolStripMenuItem
            // 
            this.operatorToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.subtractToolStripMenuItem,
            this.multiplyToolStripMenuItem,
            this.divideToolStripMenuItem,
            this.modulusToolStripMenuItem,
            this.logicalORToolStripMenuItem,
            this.logicalANDToolStripMenuItem});
            this.operatorToolStripMenuItem.Name = "operatorToolStripMenuItem";
            this.operatorToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.operatorToolStripMenuItem.Text = "Operator";
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.addToolStripMenuItem.Text = "Add";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // subtractToolStripMenuItem
            // 
            this.subtractToolStripMenuItem.Name = "subtractToolStripMenuItem";
            this.subtractToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.subtractToolStripMenuItem.Text = "Subtract";
            this.subtractToolStripMenuItem.Click += new System.EventHandler(this.subtractToolStripMenuItem_Click);
            // 
            // multiplyToolStripMenuItem
            // 
            this.multiplyToolStripMenuItem.Name = "multiplyToolStripMenuItem";
            this.multiplyToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.multiplyToolStripMenuItem.Text = "Multiply";
            this.multiplyToolStripMenuItem.Click += new System.EventHandler(this.multiplyToolStripMenuItem_Click);
            // 
            // divideToolStripMenuItem
            // 
            this.divideToolStripMenuItem.Name = "divideToolStripMenuItem";
            this.divideToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.divideToolStripMenuItem.Text = "Divide";
            this.divideToolStripMenuItem.Click += new System.EventHandler(this.divideToolStripMenuItem_Click);
            // 
            // modulusToolStripMenuItem
            // 
            this.modulusToolStripMenuItem.Name = "modulusToolStripMenuItem";
            this.modulusToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.modulusToolStripMenuItem.Text = "Modulus";
            this.modulusToolStripMenuItem.Click += new System.EventHandler(this.modulusToolStripMenuItem_Click);
            // 
            // logicalORToolStripMenuItem
            // 
            this.logicalORToolStripMenuItem.Name = "logicalORToolStripMenuItem";
            this.logicalORToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.logicalORToolStripMenuItem.Text = "Logical OR";
            this.logicalORToolStripMenuItem.Click += new System.EventHandler(this.logicalORToolStripMenuItem_Click);
            // 
            // logicalANDToolStripMenuItem
            // 
            this.logicalANDToolStripMenuItem.Name = "logicalANDToolStripMenuItem";
            this.logicalANDToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.logicalANDToolStripMenuItem.Text = "Logical AND";
            this.logicalANDToolStripMenuItem.Click += new System.EventHandler(this.logicalANDToolStripMenuItem_Click);
            // 
            // functionWrapToolStripMenuItem
            // 
            this.functionWrapToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.parenthesesToolStripMenuItem,
            this.absoluteToolStripMenuItem,
            this.roundingToolStripMenuItem,
            this.negativeToolStripMenuItem,
            this.rootsToolStripMenuItem,
            this.exponentToolStripMenuItem,
            this.suffixToolStripMenuItem});
            this.functionWrapToolStripMenuItem.Name = "functionWrapToolStripMenuItem";
            this.functionWrapToolStripMenuItem.Size = new System.Drawing.Size(97, 20);
            this.functionWrapToolStripMenuItem.Text = "Function Wrap";
            // 
            // parenthesesToolStripMenuItem
            // 
            this.parenthesesToolStripMenuItem.Name = "parenthesesToolStripMenuItem";
            this.parenthesesToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.parenthesesToolStripMenuItem.Text = "Parentheses";
            this.parenthesesToolStripMenuItem.Click += new System.EventHandler(this.parenthesesToolStripMenuItem_Click);
            // 
            // absoluteToolStripMenuItem
            // 
            this.absoluteToolStripMenuItem.Name = "absoluteToolStripMenuItem";
            this.absoluteToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.absoluteToolStripMenuItem.Text = "Absolute";
            this.absoluteToolStripMenuItem.Click += new System.EventHandler(this.absoluteToolStripMenuItem_Click);
            // 
            // roundingToolStripMenuItem
            // 
            this.roundingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.roundUpToolStripMenuItem,
            this.roundDownToolStripMenuItem});
            this.roundingToolStripMenuItem.Name = "roundingToolStripMenuItem";
            this.roundingToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.roundingToolStripMenuItem.Text = "Rounding";
            // 
            // roundUpToolStripMenuItem
            // 
            this.roundUpToolStripMenuItem.Name = "roundUpToolStripMenuItem";
            this.roundUpToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.roundUpToolStripMenuItem.Text = "Round Up";
            this.roundUpToolStripMenuItem.Click += new System.EventHandler(this.roundUpToolStripMenuItem_Click);
            // 
            // roundDownToolStripMenuItem
            // 
            this.roundDownToolStripMenuItem.Name = "roundDownToolStripMenuItem";
            this.roundDownToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.roundDownToolStripMenuItem.Text = "Round Down";
            this.roundDownToolStripMenuItem.Click += new System.EventHandler(this.roundDownToolStripMenuItem_Click);
            // 
            // negativeToolStripMenuItem
            // 
            this.negativeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sineToolStripMenuItem,
            this.cosineToolStripMenuItem,
            this.tangentToolStripMenuItem,
            this.hyperbolicToolStripMenuItem,
            this.arcToolStripMenuItem});
            this.negativeToolStripMenuItem.Name = "negativeToolStripMenuItem";
            this.negativeToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.negativeToolStripMenuItem.Text = "Trigonometry";
            // 
            // sineToolStripMenuItem
            // 
            this.sineToolStripMenuItem.Name = "sineToolStripMenuItem";
            this.sineToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.sineToolStripMenuItem.Text = "Sine";
            this.sineToolStripMenuItem.Click += new System.EventHandler(this.sineToolStripMenuItem_Click);
            // 
            // cosineToolStripMenuItem
            // 
            this.cosineToolStripMenuItem.Name = "cosineToolStripMenuItem";
            this.cosineToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.cosineToolStripMenuItem.Text = "Cosine";
            this.cosineToolStripMenuItem.Click += new System.EventHandler(this.cosineToolStripMenuItem_Click);
            // 
            // tangentToolStripMenuItem
            // 
            this.tangentToolStripMenuItem.Name = "tangentToolStripMenuItem";
            this.tangentToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.tangentToolStripMenuItem.Text = "Tangent";
            this.tangentToolStripMenuItem.Click += new System.EventHandler(this.tangentToolStripMenuItem_Click);
            // 
            // hyperbolicToolStripMenuItem
            // 
            this.hyperbolicToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sineHyperbolicToolStripMenuItem,
            this.cosineHyperbolicToolStripMenuItem,
            this.tangentHyperBolicToolStripMenuItem});
            this.hyperbolicToolStripMenuItem.Name = "hyperbolicToolStripMenuItem";
            this.hyperbolicToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.hyperbolicToolStripMenuItem.Text = "Hyperbolic";
            // 
            // sineHyperbolicToolStripMenuItem
            // 
            this.sineHyperbolicToolStripMenuItem.Name = "sineHyperbolicToolStripMenuItem";
            this.sineHyperbolicToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.sineHyperbolicToolStripMenuItem.Text = "Sine Hyperbolic";
            this.sineHyperbolicToolStripMenuItem.Click += new System.EventHandler(this.sineHyperbolicToolStripMenuItem_Click);
            // 
            // cosineHyperbolicToolStripMenuItem
            // 
            this.cosineHyperbolicToolStripMenuItem.Name = "cosineHyperbolicToolStripMenuItem";
            this.cosineHyperbolicToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.cosineHyperbolicToolStripMenuItem.Text = "Cosine Hyperbolic";
            this.cosineHyperbolicToolStripMenuItem.Click += new System.EventHandler(this.cosineHyperbolicToolStripMenuItem_Click);
            // 
            // tangentHyperBolicToolStripMenuItem
            // 
            this.tangentHyperBolicToolStripMenuItem.Name = "tangentHyperBolicToolStripMenuItem";
            this.tangentHyperBolicToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.tangentHyperBolicToolStripMenuItem.Text = "Tangent HyperBolic";
            this.tangentHyperBolicToolStripMenuItem.Click += new System.EventHandler(this.tangentHyperBolicToolStripMenuItem_Click);
            // 
            // arcToolStripMenuItem
            // 
            this.arcToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.arcSineToolStripMenuItem,
            this.arcCosineToolStripMenuItem,
            this.arcTangentToolStripMenuItem});
            this.arcToolStripMenuItem.Name = "arcToolStripMenuItem";
            this.arcToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.arcToolStripMenuItem.Text = "Arc";
            // 
            // arcSineToolStripMenuItem
            // 
            this.arcSineToolStripMenuItem.Name = "arcSineToolStripMenuItem";
            this.arcSineToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.arcSineToolStripMenuItem.Text = "Arc Sine";
            this.arcSineToolStripMenuItem.Click += new System.EventHandler(this.arcSineToolStripMenuItem_Click);
            // 
            // arcCosineToolStripMenuItem
            // 
            this.arcCosineToolStripMenuItem.Name = "arcCosineToolStripMenuItem";
            this.arcCosineToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.arcCosineToolStripMenuItem.Text = "Arc Cosine";
            this.arcCosineToolStripMenuItem.Click += new System.EventHandler(this.arcCosineToolStripMenuItem_Click);
            // 
            // arcTangentToolStripMenuItem
            // 
            this.arcTangentToolStripMenuItem.Name = "arcTangentToolStripMenuItem";
            this.arcTangentToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.arcTangentToolStripMenuItem.Text = "Arc Tangent";
            this.arcTangentToolStripMenuItem.Click += new System.EventHandler(this.arcTangentToolStripMenuItem_Click);
            // 
            // rootsToolStripMenuItem
            // 
            this.rootsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.naturalLogarithmToolStripMenuItem,
            this.base10LogarithmToolStripMenuItem});
            this.rootsToolStripMenuItem.Name = "rootsToolStripMenuItem";
            this.rootsToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.rootsToolStripMenuItem.Text = "Roots";
            // 
            // naturalLogarithmToolStripMenuItem
            // 
            this.naturalLogarithmToolStripMenuItem.Name = "naturalLogarithmToolStripMenuItem";
            this.naturalLogarithmToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.naturalLogarithmToolStripMenuItem.Text = "Natural Logarithm";
            this.naturalLogarithmToolStripMenuItem.Click += new System.EventHandler(this.naturalLogarithmToolStripMenuItem_Click);
            // 
            // base10LogarithmToolStripMenuItem
            // 
            this.base10LogarithmToolStripMenuItem.Name = "base10LogarithmToolStripMenuItem";
            this.base10LogarithmToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.base10LogarithmToolStripMenuItem.Text = "Base 10 Logarithm";
            this.base10LogarithmToolStripMenuItem.Click += new System.EventHandler(this.base10LogarithmToolStripMenuItem_Click);
            // 
            // exponentToolStripMenuItem
            // 
            this.exponentToolStripMenuItem.Name = "exponentToolStripMenuItem";
            this.exponentToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.exponentToolStripMenuItem.Text = "Exponent";
            this.exponentToolStripMenuItem.Click += new System.EventHandler(this.exponentToolStripMenuItem_Click);
            // 
            // suffixToolStripMenuItem
            // 
            this.suffixToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.makeOrdinalToolStripMenuItem});
            this.suffixToolStripMenuItem.Name = "suffixToolStripMenuItem";
            this.suffixToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.suffixToolStripMenuItem.Text = "Suffix";
            // 
            // makeOrdinalToolStripMenuItem
            // 
            this.makeOrdinalToolStripMenuItem.Name = "makeOrdinalToolStripMenuItem";
            this.makeOrdinalToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.makeOrdinalToolStripMenuItem.Text = "Make Ordinal";
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.clearToolStripMenuItem.Text = "Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(45, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // txtLabel
            // 
            this.txtLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLabel.Location = new System.Drawing.Point(286, 57);
            this.txtLabel.Name = "txtLabel";
            this.txtLabel.Size = new System.Drawing.Size(167, 26);
            this.txtLabel.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(282, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 20);
            this.label1.TabIndex = 18;
            this.label1.Text = "Label:";
            // 
            // txtResult
            // 
            this.txtResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtResult.Location = new System.Drawing.Point(12, 223);
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtResult.Size = new System.Drawing.Size(441, 107);
            this.txtResult.TabIndex = 19;
            this.txtResult.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblEvaluatesTo
            // 
            this.lblEvaluatesTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblEvaluatesTo.AutoSize = true;
            this.lblEvaluatesTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEvaluatesTo.Location = new System.Drawing.Point(8, 200);
            this.lblEvaluatesTo.Name = "lblEvaluatesTo";
            this.lblEvaluatesTo.Size = new System.Drawing.Size(59, 20);
            this.lblEvaluatesTo.TabIndex = 20;
            this.lblEvaluatesTo.Text = "Result:";
            // 
            // ticker
            // 
            this.ticker.Interval = 500;
            this.ticker.Tick += new System.EventHandler(this.ticker_Tick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(13, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 20);
            this.label2.TabIndex = 21;
            this.label2.Text = "Expression:";
            // 
            // ExprsEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(470, 365);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblEvaluatesTo);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtLabel);
            this.Controls.Add(this.txtExpression);
            this.Controls.Add(this.lblEditType);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOkay);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ExprsEdit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Expression Editor";
            this.Load += new System.EventHandler(this.ExprsEdit_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtExpression;
        private System.Windows.Forms.Label lblEditType;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOkay;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem insertToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tagNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem globalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem todaysDateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem literalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem textToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem textToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem truthToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem trueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem falseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem operatorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem subtractToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem multiplyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem divideToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modulusToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logicalORToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logicalANDToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.TextBox txtLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.Label lblEvaluatesTo;
        private System.Windows.Forms.ToolStripMenuItem functionWrapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem parenthesesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem absoluteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem roundingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem roundUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem roundDownToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem negativeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cosineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tangentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hyperbolicToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sineHyperbolicToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cosineHyperbolicToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tangentHyperBolicToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem arcToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem arcSineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem arcCosineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem arcTangentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rootsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem naturalLogarithmToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem base10LogarithmToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exponentToolStripMenuItem;
        private System.Windows.Forms.Timer ticker;
        private System.Windows.Forms.ToolStripMenuItem randomNumberToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem anyFieldNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem suffixToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem makeOrdinalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem docCommandToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteRowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteColumnToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem shrinkRowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteTableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteParagraphToolStripMenuItem;
        private System.Windows.Forms.Label label2;
    }
}