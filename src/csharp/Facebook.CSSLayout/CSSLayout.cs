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

	public class CSSLayout
	{
		public float X;
		public float Y;
		public float Width = CSSConstants.Undefined;
		public float Height = CSSConstants.Undefined;

		internal float x { get { return X; } set { X = value; } }
		internal float y { get { return Y; } set { Y = value; } }
		internal float width { get { return Width; } set { Width = value; } }
		internal float height { get { return Height; } set { Height = value; } }

		/**
		* This should always get called before calling {@link LayoutEngine#layoutNode(CSSNode, float)}
		*/

		public void resetResult()
		{
			X = 0;
			y = 0;
			Width = CSSConstants.Undefined;
			Height = CSSConstants.Undefined;
		}

		public void copy(CSSLayout layout)
		{
			X = layout.X;
			y = layout.y;
			Width = layout.Width;
			Height = layout.Height;
		}

		public override string ToString()
		{
			return "Layout: {" +
					"X: " + X + ", " +
					"Y: " + y + ", " +
					"Width: " + Width + ", " +
					"Height: " + Height +
					"}";
		}
	}
}
