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
	 * The CSS style definition for a {@link CSSNode}.
	 */
	class CSSStyle
	{
		public CSSFlexDirection flexDirection = CSSFlexDirection.Column;
		public CSSJustify justifyContent = CSSJustify.FlexStart;
		public CSSAlign alignItems = CSSAlign.Stretch;
		public CSSAlign alignSelf = CSSAlign.Auto;
		public CSSPositionType positionType = CSSPositionType.RELATIVE;
		public CSSWrap flexWrap = CSSWrap.NoWrap;
		public float flex;

		public float[] margin = Spacing.newSpacingResultArray();
		public float[] padding = Spacing.newSpacingResultArray();
		public float[] border = Spacing.newSpacingResultArray();

		public float positionTop = CSSConstants.Undefined;
		public float positionBottom = CSSConstants.Undefined;
		public float positionLeft = CSSConstants.Undefined;
		public float positionRight = CSSConstants.Undefined;

		public float width = CSSConstants.Undefined;
		public float height = CSSConstants.Undefined;

		public float minWidth = CSSConstants.Undefined;
		public float minHeight = CSSConstants.Undefined;

		public float maxWidth = CSSConstants.Undefined;
		public float maxHeight = CSSConstants.Undefined;
	}
}

