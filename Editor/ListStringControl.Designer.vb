﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ListStringControl
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
        Me.ctlListEditor = New AxeSoftware.Quest.ListEditor()
        Me.SuspendLayout()
        '
        'ctlListEditor
        '
        Me.ctlListEditor.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ctlListEditor.Location = New System.Drawing.Point(0, 0)
        Me.ctlListEditor.Name = "ctlListEditor"
        Me.ctlListEditor.Size = New System.Drawing.Size(474, 105)
        Me.ctlListEditor.TabIndex = 0
        '
        'ListStringControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.ctlListEditor)
        Me.Name = "ListStringControl"
        Me.Size = New System.Drawing.Size(474, 105)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ctlListEditor As AxeSoftware.Quest.ListEditor

End Class