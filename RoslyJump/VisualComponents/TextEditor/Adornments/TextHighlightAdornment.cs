using System;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace RoslyJump.VisualComponents.TextEditor.Adornments
{
    /// <summary>
    /// Highlights a specified text range with a purple box.
    /// </summary>
    internal sealed class TextHighlightAdornment
    {
        /// <summary>
        /// The layer of the adornment.
        /// </summary>
        private readonly IAdornmentLayer layer;

        /// <summary>
        /// Text view where the adornment is created.
        /// </summary>
        private readonly IWpfTextView view;

        /// <summary>
        /// Adornment brush.
        /// </summary>
        private readonly Brush brush;

        /// <summary>
        /// Adornment pen.
        /// </summary>
        private readonly Pen pen;

        private bool applied = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextHighlightAdornment"/> class.
        /// </summary>
        /// <param name="view">Text view to create the adornment for</param>
        public TextHighlightAdornment(IWpfTextView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            layer = view.GetAdornmentLayer("TextHighlightAdornment");

            this.view = view;
            this.view.LayoutChanged += OnLayoutChanged;

            // Create the pen and brush to color the box behind the a's
            brush = new SolidColorBrush(Color.FromArgb(0x20, 0x00, 0x00, 0xff));
            brush.Freeze();

            var penBrush = new SolidColorBrush(Colors.Red);
            penBrush.Freeze();
            pen = new Pen(penBrush, 0.5);
            pen.Freeze();
        }

        /// <summary>
        /// Handles whenever the text displayed in the view changes by adding the adornment to any reformatted lines
        /// </summary>
        /// <remarks><para>This event is raised whenever the rendered text displayed in the <see cref="ITextView"/> changes.</para>
        /// <para>It is raised whenever the view does a layout (which happens when DisplayTextLineContainingBufferPosition is called or in response to text or classification changes).</para>
        /// <para>It is also raised whenever the view scrolls horizontally or when its size changes.</para>
        /// </remarks>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        internal void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            SnapshotPoint caretPositionInBuffer = view.Caret.Position.BufferPosition;

            IWpfTextViewLine line =
                view.GetTextViewLineContainingBufferPosition(caretPositionInBuffer);

            if (e.NewOrReformattedLines.Contains(line))
            {
                // this.CreateVisuals(line);
            }

            //foreach (ITextViewLine line in e.NewOrReformattedLines)
            //{
            //    this.CreateVisuals(line);
            //}
        }

        public void EndorseActiveLine()
        {
            if (applied) throw new InvalidOperationException("This adornment is already applied.");
            SnapshotPoint caretPositionInBuffer = view.Caret.Position.BufferPosition;
            IWpfTextViewLine line =
                view.GetTextViewLineContainingBufferPosition(caretPositionInBuffer);

            CreateVisuals(line);
            applied = true;
        }

        internal void EndorseLine(int line, int charStart, int charEnd)
        {
            if (applied) throw new InvalidOperationException("This adornment is already applied.");

            ITextSnapshot textSnapshot = view.TextSnapshot;
            ITextSnapshotLine textViewStartLine = textSnapshot.GetLineFromLineNumber(line);

            SnapshotSpan span = new SnapshotSpan(
                view.TextSnapshot,
                Span.FromBounds(
                    textViewStartLine.Start + charStart,
                    textViewStartLine.Start + charEnd));

            IWpfTextViewLineCollection textViewLines = view.TextViewLines;
            Geometry geometry = textViewLines.GetMarkerGeometry(span);
            if (geometry != null)
            {
                var drawing = new GeometryDrawing(brush, pen, geometry);
                drawing.Freeze();

                var drawingImage = new DrawingImage(drawing);
                drawingImage.Freeze();

                var image = new Image
                {
                    Source = drawingImage,
                };

                // Align the image with the top of the bounds of the text geometry
                Canvas.SetLeft(image, geometry.Bounds.Left);
                Canvas.SetTop(image, geometry.Bounds.Top);

                layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, this, image, null);
            }

            applied = true;
        }

        internal void EndorseTextBounds(int lineStart, int lineEnd, int charStart, int charEnd)
        {
            if (applied) throw new InvalidOperationException("This adornment is already applied.");

            ITextSnapshot textSnapshot = view.TextSnapshot;
            ITextSnapshotLine textViewStartLine = textSnapshot.GetLineFromLineNumber(lineStart);
            ITextSnapshotLine textViewEndLine = textSnapshot.GetLineFromLineNumber(lineEnd);

            SnapshotSpan span = new SnapshotSpan(
                view.TextSnapshot,
                Span.FromBounds(
                    textViewStartLine.Start + charStart,
                    textViewEndLine.Start + charEnd));

            IWpfTextViewLineCollection textViewLines = view.TextViewLines;
            Geometry geometry = textViewLines.GetMarkerGeometry(span);
            if (geometry != null)
            {
                var drawing = new GeometryDrawing(brush, pen, geometry);
                drawing.Freeze();

                var drawingImage = new DrawingImage(drawing);
                drawingImage.Freeze();

                var image = new Image
                {
                    Source = drawingImage,
                };

                // Align the image with the top of the bounds of the text geometry
                Canvas.SetLeft(image, geometry.Bounds.Left);
                Canvas.SetTop(image, geometry.Bounds.Top);

                layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, this, image, null);
            }

            applied = true;
        }

        public void Remove()
        {
            layer.RemoveAdornmentsByTag(this);
            applied = false;
        }

        /// <summary>
        /// Adds the scarlet box behind the 'a' characters within the given line
        /// </summary>
        /// <param name="line">Line to add the adornments</param>
        private void CreateVisuals(ITextViewLine line)
        {
            IWpfTextViewLineCollection textViewLines = view.TextViewLines;

            // Loop through each character, and place a box around any 'a'
            for (int charIndex = line.Start; charIndex < line.End; charIndex++)
            {
                if (view.TextSnapshot[charIndex] == 'a')
                {
                    SnapshotSpan span = new SnapshotSpan(view.TextSnapshot, Span.FromBounds(charIndex, charIndex + 1));
                    Geometry geometry = textViewLines.GetMarkerGeometry(span);
                    if (geometry != null)
                    {
                        var drawing = new GeometryDrawing(brush, pen, geometry);
                        drawing.Freeze();

                        var drawingImage = new DrawingImage(drawing);
                        drawingImage.Freeze();

                        var image = new Image
                        {
                            Source = drawingImage,
                        };

                        // Align the image with the top of the bounds of the text geometry
                        Canvas.SetLeft(image, geometry.Bounds.Left);
                        Canvas.SetTop(image, geometry.Bounds.Top);

                        layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, this, image, null);
                    }
                }
            }
        }
    }
}
