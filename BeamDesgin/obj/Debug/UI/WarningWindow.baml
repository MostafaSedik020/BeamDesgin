   M S B A M L   `   `   `  ����E  ABeamDesgin, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:BeamDesgin.UIBeamDesgin.UI  5      !    BeamDesgin.UI.WarningWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      -localclr-namespace:BeamDesgin.UI     5      -   5        $	��400��5      $	��500��6     � Title$  Warning Window��5
   	   
������  � WindowStyle$
 None=�5   	    � AllowsTransparency . .��5      ��  �� CornerRadius$ 25��6   $��1q�6   
������ 
������ ._�5   
   � ��5      �� 5       �� 
StartPoint$	 0,0&�6&    �� EndPoint$	 1,1&�67   .��5      �  � Color$ #e9edf5��6#    � Offset$	 0.0Q�63   � 5      $ #cdd9ea��6#   $	 0.5Q�63   � 5      $ #e9edf5��6#   $	 1.0Q�63   5       � RowDefinitions 5      �� 5      $��30 �6    6.   �� 5      $��* �6    6-   .+�5      � $��0��6   
������ 	 � ColumnDefinitions	 5      �� 5      $��* �6'   63   �� 5       $
��Auto �6'   66   .+�5"      �� $��Warnings��6   $��Center=�6+   $��10,0,0,0q�6F   $��16(�6X   $
��Bold"�5#      $��Arial)�6+   
���   �$��0��6Q   �� 5%      -   5        $��X��5%      $��30��6%   $��30��60   
������ 
������ 
���   �$��1��5&   F   5'      �� 5(      -   5        
 ��Name$
 Warning_TxtBox��5(      $��1��6.    �� 
IsReadOnly .  �� AcceptsReturn . " �� VerticalScrollBarVisibility$ Visible=�6b   5)      5+      5-      