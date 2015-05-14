/**
 * Copyright (c) 2014, Facebook, Inc.
 * All rights reserved.
 *
 * This source code is licensed under the BSD-style license found in the
 * LICENSE file in the root directory of this source tree. An additional grant
 * of patent rights can be found in the PATENTS file in the same directory.
 */

namespace Facebook.CSSLayout
{

	/**
	 * Where the output of {@link LayoutEngine#layoutNode(CSSNode, float)} will go in the CSSNode.
	 */

	class CSSLayout
	{
		public float Left;
		public float Top;
		public float Right;
		public float Bottom;
		public float Width = CSSConstants.Undefined;
		public float Height = CSSConstants.Undefined;

		internal float left { get { return Left; } set { Left = value; } }
		internal float top { get { return Top; } set { Top = value; } }
		internal float right { get { return Right; } set { Right = value; } }
		internal float bottom { get { return Bottom; } set { Bottom = value; } }
		internal float width { get { return Width; } set { Width = value; } }
		internal float height { get { return Height; } set { Height = value; } }

		/**
		* This should always get called before calling {@link LayoutEngine#layoutNode(CSSNode, float)}
		*/

		public void resetResult()
		{
			Left = 0;
			Top = 0;
			Right = 0;
			Bottom = 0;
			Width = CSSConstants.Undefined;
			Height = CSSConstants.Undefined;
		}

		public void copy(CSSLayout layout)
		{
			Left = layout.Left;
			Top = layout.Top;
			Right = layout.Right;
			Bottom = layout.Bottom;
			Width = layout.Width;
			Height = layout.Height;
		}

		public override string ToString()
		{
			return "Layout: {" +
					"Left: " + Left + ", " +
					"Top: " + Top + ", " +
					"Width: " + Width + ", " +
					"Height: " + Height +
					"}";
		}
	}
}
