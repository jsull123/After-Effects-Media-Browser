   M S B A M L   `   `   `  ���� W  SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   PrimaryColor  PrimaryColorDark  SecondaryColor  BackgroundColor  
GreyColor3  
TextColor1  
TextColor2  PrimaryColorBrush  �� Color  PrimaryColorDarkBrush 	 SecondaryColorBrush 
 BackgroundColorBrush  GreyColor3Brush  TextColor1Brush  TextColor2Brush  ButtonStyle1 �� 
TargetType �� Property �� Value  ButtonStyle2 �� 
TargetType �� CornerRadius �� Triggers P� Property P� Value  SliderThumbStyle	 A�Name'   !System.Windows.VisualStateManager
    VisualStateGroups%  System.Windows.VisualStateGroup   States   System.Windows.VisualState   
Storyboard �� TargetProperty �� 
TargetName	 �� To g� Duration  HorizontalSlider � RowDefinitions ��Name ]�Name ]� DecreaseRepeatButton ]� Thumb ]� IncreaseRepeatButton �� Triggers  ScrollBarThumb �� Binding �� RelativeSource �� AncestorType �� Path �� Value  VerticalScrollBar  HorizontalScrollBar � ColumnDefinitions  StripedProgressBarStyle �Name g� RepeatBehavior  � From	! � To" ��Name# j� TileMode$ j� Stretch% j� Viewport& j� ViewportUnits' L� Drawing( � Brush) � Geometry* E� 
StartPoint+ �� Point, �� 
TargetName�� F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      :x,http://schemas.microsoft.com/winfx/2006/xaml      %�  &	        &	      &	 "     &	 3     &	 D     &	 U     &	 f     &	 t     7   &	 ~     7  &		 �     7  &	
 �     7  &	 �     7  &	 �     7  &	 �     7  &	 �     7  7  &	     7  7  7  &	 H    7   &	 1    7  7  '�� K    7  &	 "    &	 �    7  7  &	 	    7  7  '�� '
    7  7  &	 j    7  7  7  ��#2D8CEB����#1472d1����#2b2b2b����#232323����#3b3b3b����#EEEEEE����Gray���� 8    �� 8    �� 8    �� 8    �� 8    �� 8    �� 8    �� " ��.���  � ��8   ��  � ��8  ��  � ��
 ���� ��  � ��$ 0q��� " ��.���  � ��8   ��  � ��8  ��  � ��
 ���� ��  � ��$ 0q���  � �� �� " ��.R��� #��z��#��z��#��z��$ 5��._��� $��Center=�$��Center=� P�  �@J�IsMouseOver$
 True��. ��� 8   � ��P�  � ��$
 True��. ���  � ��
 ������� " n�.���  �@J�SnapsToDevicePixels$
 true����  �@�OverridesDefaultStyle$
 true����  � ��$ 14����  � ��$ 14����  � �� �� " n�.R�A� $	 Ellipse������ 8    
   CommonStates��.   
Normal��  	MouseOver��. �� .���� $* $(Shape.Fill).(SolidColorBrush.Color)�$ Ellipse�� #1472d1$ 0:0:0.2D�  Pressed��. �� .���� $* $(Shape.Fill).(SolidColorBrush.Color)�$ Ellipse�� #0f59a3$ 0:0:0.2D��� " ��.R��  �� $
��Auto ��� $
��Auto �#��z���� $
��Auto �.+��� $ TrackBackground��$��4��$ 2��8��  $��0,0,0,0q�$��1��]� $ 
PART_Track��$��1�� �� $��0��$��Slider.DecreaseLarge�� n� 8��  �� $��0��$��Slider.IncreaseLarge���� " ��.���  �@J�SnapsToDevicePixels$
 true����  �@�OverridesDefaultStyle$
 true�� P�  �@��Orientation$ 
Horizontal=�. ���  � ��$	 104����  � ��$ 21����  � ��8   �� " n�.���  �@J�SnapsToDevicePixels$
 True����  �@�OverridesDefaultStyle$
 true����  � ��$ false����  � |�$ false����  � �� �� " n�.R��� $ 4��#��z��#��z��$��0q����� " �� ��  ��  ��  	ScrollBar$ Orientation� 
Horizontal.a���  � ��$ 8����  � ��$ 0,0,0,0q���  ��  ��  	ScrollBar$ Orientation� Vertical.a���  � ��$ 8����  � ��$ 8,0,-2,0q��� " ��.R��  �� $��0���� $��0.00001* ��� $��0��.+��� $��3��$ 2��
������ ]� $��
PART_Track��$��1��$
��true�� �� $��0��$��ScrollBar.PageUpCommand�� n� 8��  $��1,0,1,0q�8��  �� $��0��$��ScrollBar.PageDownCommand���� " ��.R��  �� $��18���� $��0.00001* ��� $��18��.+��� $��3��$ 2��
������ ]� $��
PART_Track��$��1��$��False�� �� $��0��$��ScrollBar.PageLeftCommand�� n� 8��  $��1,0,1,0q�8��  �� $��0��$ ��ScrollBar.PageRightCommand���� " ��.���  �@J�SnapsToDevicePixels$
 True����  �@�OverridesDefaultStyle$
 true�� P�  �@��Orientation$ 
Horizontal=�. ���  � ��$
 Auto����  � ��$ 18����  � ��8   P�  �@��Orientation$ Vertical=�. ���  � ��$ 18����  � ��$
 Auto����  � ��8  �� " �.���  � ��8   ��  � ��8  ��  � ��$ 20����  � �� �� " �.R�� $ TemplateRoot��
   CommonStates��.   Determinate��. �� $ Forever��.��� $ Stripe��$' !(Shape.Fill).(TileBrush.Viewport)�$ 0:0:1D�  -20,0,20,20! 	0,0,20,20  Indeterminate��.+��� #��z���� $" 
PART_Track��� $ PART_Indicator��$
}�true��$
��Left=�.+��� $" 	Indicator��#��z���� $" Stripe����L� $
# Tile=�$$ Uniform=�$% 	0,0,20,20�$& Absolute=�' � 8(  ) B� ��D� E� $	* 0,0&�.���� $	+ 5,0&��� $
+ 10,5&��� $+ 10,10&�E� $	* 0,5&�.���� $
+ 5,10&��� $
+ 0,10&� P�  �@�Value$	 100Q�. ��� $, Stripe�� � y�$ 	Collapsed=�