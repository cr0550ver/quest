﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class RichTextControl
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(RichTextControl))
        Me.ctlToolbar = New System.Windows.Forms.ToolStrip()
        Me.butBold = New System.Windows.Forms.ToolStripButton()
        Me.butItalic = New System.Windows.Forms.ToolStripButton()
        Me.butUnderline = New System.Windows.Forms.ToolStripButton()
        Me.ctlWebBrowser = New System.Windows.Forms.WebBrowser()
        Me.ctlToolbar.SuspendLayout()
        Me.SuspendLayout()
        '
        'ctlToolbar
        '
        Me.ctlToolbar.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.butBold, Me.butItalic, Me.butUnderline})
        Me.ctlToolbar.Location = New System.Drawing.Point(0, 0)
        Me.ctlToolbar.Name = "ctlToolbar"
        Me.ctlToolbar.Size = New System.Drawing.Size(469, 25)
        Me.ctlToolbar.TabIndex = 0
        Me.ctlToolbar.Text = "ToolStrip1"
        '
        'butBold
        '
        Me.butBold.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.butBold.Image = CType(resources.GetObject("butBold.Image"), System.Drawing.Image)
        Me.butBold.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.butBold.Name = "butBold"
        Me.butBold.Size = New System.Drawing.Size(23, 22)
        Me.butBold.Text = "Bold"
        '
        'butItalic
        '
        Me.butItalic.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.butItalic.Image = CType(resources.GetObject("butItalic.Image"), System.Drawing.Image)
        Me.butItalic.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.butItalic.Name = "butItalic"
        Me.butItalic.Size = New System.Drawing.Size(23, 22)
        Me.butItalic.Text = "Italic"
        '
        'butUnderline
        '
        Me.butUnderline.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.butUnderline.Image = CType(resources.GetObject("butUnderline.Image"), System.Drawing.Image)
        Me.butUnderline.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.butUnderline.Name = "butUnderline"
        Me.butUnderline.Size = New System.Drawing.Size(23, 22)
        Me.butUnderline.Text = "Underline"
        '
        'ctlWebBrowser
        '
        Me.ctlWebBrowser.AllowWebBrowserDrop = False
        Me.ctlWebBrowser.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ctlWebBrowser.IsWebBrowserContextMenuEnabled = False
        Me.ctlWebBrowser.Location = New System.Drawing.Point(0, 25)
        Me.ctlWebBrowser.MinimumSize = New System.Drawing.Size(20, 20)
        Me.ctlWebBrowser.Name = "ctlWebBrowser"
        Me.ctlWebBrowser.Size = New System.Drawing.Size(469, 238)
        Me.ctlWebBrowser.TabIndex = 1
        '
        'RichTextControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.ctlWebBrowser)
        Me.Controls.Add(Me.ctlToolbar)
        Me.Name = "RichTextControl"
        Me.Size = New System.Drawing.Size(469, 263)
        Me.ctlToolbar.ResumeLayout(False)
        Me.ctlToolbar.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ctlToolbar As System.Windows.Forms.ToolStrip
    Friend WithEvents butBold As System.Windows.Forms.ToolStripButton
    Friend WithEvents ctlWebBrowser As System.Windows.Forms.WebBrowser
    Friend WithEvents butItalic As System.Windows.Forms.ToolStripButton
    Friend WithEvents butUnderline As System.Windows.Forms.ToolStripButton

End Class