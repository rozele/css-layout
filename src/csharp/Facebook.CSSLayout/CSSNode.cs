/**
 * Copyright (c) 2014, Facebook, Inc.
 * All rights reserved.
 *
 * This source code is licensed under the BSD-style license found in the
 * LICENSE file in the root directory of this source tree. An additional grant
 * of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Facebook.CSSLayout
{
	/**
	 * A CSS Node. It has a style object you can manipulate at {@link #style}. After calling
	 * {@link #calculateLayout()}, {@link #layout} will be filled with the results of the layout.
	 */

	public class CSSNode
	{
		enum LayoutState
		{
			/**
		 * Some property of this node or its children has changes and the current values in
		 * {@link #layout} are not valid.
		 */
			DIRTY,

			/**
		 * This node has a new layout relative to the last time {@link #markLayoutSeen()} was called.
		 */
			HAS_NEW_LAYOUT,

			/**
		 * {@link #layout} is valid for the node's properties and this layout has been marked as
		 * having been seen.
		 */
			UP_TO_DATE,
		}

		// Only one copy kept around to keep from allocating a bunch of MeasureOutput objects
		// NOT THREAD SAFE! NOT RE-ENTRANT SAFE!
		static readonly MeasureOutput MEASURE_OUTPUT = new MeasureOutput();

		/**
		 * Should measure the given node and put the result in the given MeasureOutput.
		 *
		 * NB: measure is NOT guaranteed to be threadsafe/re-entrant safe!
		 */
		public delegate void MeasureFunction(CSSNode node, float width, MeasureOutput measureOutput);

		readonly float[] mMargin = Spacing.newFullSpacingArray();
		readonly float[] mPadding = Spacing.newFullSpacingArray();
		readonly float[] mBorder = Spacing.newFullSpacingArray();

		internal readonly CSSStyle style = new CSSStyle();
		internal readonly CSSLayout layout = new CSSLayout();
		internal readonly CachedCSSLayout lastLayout = new CachedCSSLayout();

		// 4 is kinda arbitrary, but the default of 10 seems really high for an average View.
		readonly List<CSSNode> mChildren = new List<CSSNode>(4);
		[Nullable] CSSNode mParent;
		[Nullable] MeasureFunction mMeasureFunction = null;
		LayoutState mLayoutState = LayoutState.DIRTY;

		public int getChildCount()
		{
			return mChildren.Count;
		}

		public CSSNode getChildAt(int i)
		{
			return mChildren[i];
		}

		public void addChildAt(CSSNode child, int i)
		{
			if (child.mParent != null)
			{
				throw new InvalidOperationException("Child already has a parent, it must be removed first.");
			}

			mChildren.Insert(i, child);
			child.mParent = this;
			dirty();
		}

		public void removeChildAt(int i)
		{
			mChildren[i].mParent = null;
			mChildren.RemoveAt(i);
			dirty();
		}

		[return: Nullable] 
		public CSSNode getParent()
		{
			return mParent;
		}

		/**
	   * @return the index of the given child, or -1 if the child doesn't exist in this node.
	   */

		public int indexOf(CSSNode child)
		{
			return mChildren.IndexOf(child);
		}

		public void setMeasureFunction(MeasureFunction measureFunction)
		{
			if (!valuesEqual(mMeasureFunction, measureFunction))
			{
				mMeasureFunction = measureFunction;
				dirty();
			}
		}

		public bool isMeasureDefined()
		{
			return mMeasureFunction != null;
		}

		internal MeasureOutput measure(float width)
		{
			if (!isMeasureDefined())
			{
				throw new Exception("Measure function isn't defined!");
			}
			MEASURE_OUTPUT.height = CSSConstants.UNDEFINED;
			MEASURE_OUTPUT.width = CSSConstants.UNDEFINED;
			Assertions.assertNotNull(mMeasureFunction)(this, width, MEASURE_OUTPUT);
			return MEASURE_OUTPUT;
		}

		/**
	   * Performs the actual layout and saves the results in {@link #layout}
	   */

		public void calculateLayout()
		{
			layout.resetResult();
			LayoutEngine.layoutNode(this, CSSConstants.UNDEFINED);
		}

		/**
	   * See {@link LayoutState#DIRTY}.
	   */

		internal bool isDirty()
		{
			return mLayoutState == LayoutState.DIRTY;
		}

		/**
	   * See {@link LayoutState#HAS_NEW_LAYOUT}.
	   */

		public bool hasNewLayout()
		{
			return mLayoutState == LayoutState.HAS_NEW_LAYOUT;
		}

		protected void dirty()
		{
			if (mLayoutState == LayoutState.DIRTY)
			{
				return;
			}
			else if (mLayoutState == LayoutState.HAS_NEW_LAYOUT)
			{
				throw new InvalidOperationException("Previous layout was ignored! markLayoutSeen() never called");
			}

			mLayoutState = LayoutState.DIRTY;

			if (mParent != null)
			{
				mParent.dirty();
			}
		}

		internal void markHasNewLayout()
		{
			mLayoutState = LayoutState.HAS_NEW_LAYOUT;
		}

		/**
	   * Tells the node that the current values in {@link #layout} have been seen. Subsequent calls
	   * to {@link #hasNewLayout()} will return false until this node is laid out with new parameters.
	   * You must call this each time the layout is generated if the node has a new layout.
	   */

		public void markLayoutSeen()
		{
			if (!hasNewLayout())
			{
				throw new InvalidOperationException("Expected node to have a new layout to be seen!");
			}

			mLayoutState = LayoutState.UP_TO_DATE;
		}

		void toStringWithIndentation(StringBuilder result, int level)
		{
			// Spaces and tabs are dropped by IntelliJ logcat integration, so rely on __ instead.
			StringBuilder indentation = new StringBuilder();
			for (int i = 0; i < level; ++i)
			{
				indentation.Append("__");
			}

			result.Append(indentation.ToString());
			result.Append(layout.ToString());

			if (getChildCount() == 0)
			{
				return;
			}

			result.Append(", children: [\n");
			for (var i = 0; i < getChildCount(); i++)
			{
				getChildAt(i).toStringWithIndentation(result, level + 1);
				result.Append("\n");
			}
			result.Append(indentation + "]");
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			this.toStringWithIndentation(sb, 0);
			return sb.ToString();
		}

		protected bool valuesEqual(float f1, float f2)
		{
			return FloatUtil.floatsEqual(f1, f2);
		}

		protected bool valuesEqual<T>([Nullable] T o1, [Nullable] T o2)
		{
			if (o1 == null)
			{
				return o2 == null;
			}
			return o1.Equals(o2);
		}

		public void setFlexDirection(CSSFlexDirection flexDirection)
		{
			if (!valuesEqual(style.flexDirection, flexDirection))
			{
				style.flexDirection = flexDirection;
				dirty();
			}
		}

		public void setJustifyContent(CSSJustify justifyContent)
		{
			if (!valuesEqual(style.justifyContent, justifyContent))
			{
				style.justifyContent = justifyContent;
				dirty();
			}
		}

		public void setAlignItems(CSSAlign alignItems)
		{
			if (!valuesEqual(style.alignItems, alignItems))
			{
				style.alignItems = alignItems;
				dirty();
			}
		}

		public void setAlignSelf(CSSAlign alignSelf)
		{
			if (!valuesEqual(style.alignSelf, alignSelf))
			{
				style.alignSelf = alignSelf;
				dirty();
			}
		}

		public void setPositionType(CSSPositionType positionType)
		{
			if (!valuesEqual(style.positionType, positionType))
			{
				style.positionType = positionType;
				dirty();
			}
		}

		public void setWrap(CSSWrap flexWrap)
		{
			if (!valuesEqual(style.flexWrap, flexWrap))
			{
				style.flexWrap = flexWrap;
				dirty();
			}
		}

		public void setFlex(float flex)
		{
			if (!valuesEqual(style.flex, flex))
			{
				style.flex = flex;
				dirty();
			}
		}

		public void setMargin(int spacingType, float margin)
		{
			setSpacing(mMargin, style.margin, spacingType, margin);
		}

		public void setPadding(int spacingType, float padding)
		{
			setSpacing(mPadding, style.padding, spacingType, padding);
		}

		public void setBorder(int spacingType, float border)
		{
			setSpacing(mBorder, style.border, spacingType, border);
		}

		protected void setSpacing(
			float[] spacingDef,
			float[] cssStyle,
			int spacingType,
			float spacing)
		{
			if (!valuesEqual(spacingDef[spacingType], spacing))
			{
				Spacing.updateSpacing(spacingDef, cssStyle, spacingType, spacing, 0);
				dirty();
			}
		}

		public void setPositionTop(float positionTop)
		{
			if (!valuesEqual(style.positionTop, positionTop))
			{
				style.positionTop = positionTop;
				dirty();
			}
		}

		public void setPositionBottom(float positionBottom)
		{
			if (!valuesEqual(style.positionBottom, positionBottom))
			{
				style.positionBottom = positionBottom;
				dirty();
			}
		}

		public void setPositionLeft(float positionLeft)
		{
			if (!valuesEqual(style.positionLeft, positionLeft))
			{
				style.positionLeft = positionLeft;
				dirty();
			}
		}

		public void setPositionRight(float positionRight)
		{
			if (!valuesEqual(style.positionRight, positionRight))
			{
				style.positionRight = positionRight;
				dirty();
			}
		}

		public void setStyleWidth(float width)
		{
			if (!valuesEqual(style.width, width))
			{
				style.width = width;
				dirty();
			}
		}

		public void setStyleHeight(float height)
		{
			if (!valuesEqual(style.height, height))
			{
				style.height = height;
				dirty();
			}
		}

		public float getLayoutX()
		{
			return layout.x;
		}

		public float getLayoutY()
		{
			return layout.y;
		}

		public float getLayoutWidth()
		{
			return layout.width;
		}

		public float getLayoutHeight()
		{
			return layout.height;
		}
	}
}
