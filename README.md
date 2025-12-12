# LiveSplit.GradingSplits

A LiveSplit component that shows a percentile-based grade for your current split performance, complete with distribution visualization and customizable grading thresholds.

## ⚠️ Important Warning

**Always back up your LiveSplit installation before installing or updating this component!**

This component modifies LiveSplit's internal state (split icons, names) during operation. Occasionally, updates to the DLL can cause issues with your LiveSplit installation that may require:
- Deleting and re-extracting LiveSplit from scratch
- Restoring your splits, settings, and layouts from backup

**Recommended before installing/updating:**
1. Make a copy of your entire LiveSplit folder
2. Back up your `.lss` split files
3. Back up your `.lsl` layout files
4. Back up your LiveSplit settings

## Features

- **Real-time Grading**: See how your current split compares to your historical attempts on a percentile scale (0-100)
- **Dynamic Comparison Label**: Shows which comparison you're grading against (e.g., "Personal Best's Grade:")
- **Previous Split Comparison**: See how your previous split performed vs the comparison time
- **Split Name Grades**: Optionally display grades directly in split names with customizable format
- **Grade Icons**: Display grade badges as split icons with auto-generated or custom images
- **Custom Icon Support**: Use your own images for grade icons (see Custom Icons section)
- **Customizable Thresholds**: Define your own grade labels, percentile cutoffs, and colors
- **Distribution Graph**: Optional visualization showing:
  - Normal distribution curve based on your historical data
  - Historical attempts displayed as histogram (stacked dots) or scatter plot
  - Your current attempt position
  - Comparison time reference line
  - Mean (average) time line
  - Statistical summary (Average, Percentile, sample size)
- **Special Badges**: Automatic detection and custom labels for:
  - Gold segments (personal best)
  - Worst segments
- **Background Color Options**: Apply colors to the label background for better visibility

## Installation

1. Download `LiveSplit.GradingSplits.dll` from the [releases page](https://github.com/MrTyton/LiveSplit.GradingSplits/releases) or from the root of this repository
2. Place it in your LiveSplit `Components` folder (usually `LiveSplit\Components\`)
3. Right-click LiveSplit → Edit Layout → Add → Information → Grading Splits
4. Adjust settings as desired

## How It Works

The component analyzes your split history to calculate:
1. **Mean (Average)**: The average time for each split across all attempts
2. **Standard Deviation**: How much variation exists in your times
3. **Z-Score**: How many standard deviations your current time is from the mean
4. **Percentile**: Converts the z-score to a 0-100 percentile rank
   - 0% = fastest possible (best)
   - 50% = average
   - 100% = slowest possible (worst)

Your current split time is then graded based on which percentile threshold it falls under.

## Default Grading Scale

| Grade | Percentile Range | Color | Meaning |
|-------|------------------|-------|---------|
| S | 0-7% | Gold | Exceptional (top ~7%) |
| A | 7-31% | Lime Green | Above average |
| B | 31-69% | Light Green | Average |
| C | 69-93% | Orange | Below average |
| F | 93-100% | Red | Poor performance |
| ★ | — | Gold | Personal best (gold split) |
| ✗ | — | Dark Red | Worst segment |

These are based on standard deviation intervals:
- S: Better than z=-1.5 (93rd percentile performance)
- A: Between z=-1.5 and z=-0.5
- B: Between z=-0.5 and z=+0.5
- C: Between z=+0.5 and z=+1.5
- F: Worse than z=+1.5

## Customization

### Threshold Settings
- **Add/Remove Thresholds**: Create your own grading system with as many or as few grades as you want
- **Percentile Values**: Set custom percentile cutoffs (0-100)
- **Labels**: Use any text for your grades (A-F, numbers, emoji, etc.)
- **Colors**: Choose individual colors for each grade

### Display Options
- **Background Color**: Toggle background fill on the grade label
- **Gold Badge**: Enable/disable special badge for personal best splits
  - Customize label and color
- **Worst Badge**: Enable/disable special badge for worst segments
  - Customize label and color
- **Distribution Graph**: Toggle visualization on/off
  - Adjust graph height (default: 80px)
  - Choose graph style: Histogram (stacked dots) or Scatter (line)
- **Statistics Display**: Toggle statistics text below graph
  - Adjust font size (6-24px)
- **Previous Split**: Toggle previous split comparison display
  - Shows "Previous: Achieved [grade] vs [comparison]'s [grade]"
  - Displays "Previous: N/A" when no data available
  - Adjust font size (6-24px)
- **Current Grade Display**: Show grade as icon or text
- **Split Name Grades**: Display grades directly in split names
  - Customizable format string using `{Name}` and `{Grade}` placeholders
  - Examples: `{Name} [{Grade}]`, `[{Grade}] {Name}`, `{Name} ({Grade})`
  - Shows comparison grade for upcoming splits, achieved grade for completed splits
  - Automatically reverts to original names on reset
- **Grade Icons**: Display grade badges as split icons
  - Auto-generates colored circular icons by default
  - Supports custom icons (see below)

### Custom Icons

You can use your own images for grade icons instead of the auto-generated colored circles.

**Setup:**
1. Create a folder containing your custom icon images
2. In the Grading Splits settings, click the `...` button next to "Custom Icon Folder"
3. Select any image file inside your icons folder (the folder path will be extracted)

**Icon Naming Convention:**
- Name icons to match your grade labels: `S.png`, `A.png`, `B.png`, `C.png`, `F.png`, etc.
- Use `Best.png` for gold/personal best splits
- Use `Worst.png` for worst splits
- Any grades without matching icons will use auto-generated ones

**Supported Formats:** PNG, JPG, JPEG, GIF, BMP, ICO

**Example folder structure:**
```
MyIcons/
├── S.png      (for S grade)
├── A.png      (for A grade)
├── B.png      (for B grade)
├── C.png      (for C grade)
├── F.png      (for F grade)
├── Best.png   (for gold/PB splits)
└── Worst.png  (for worst splits)
```

### Graph Visualization
When enabled, the graph shows:
- **Bell curve**: Normal distribution based on your historical data
- **Yellow dots**: Your previous attempts for this split (histogram or scatter style)
- **Red vertical line**: Your comparison time (e.g., PB pace)
- **Green vertical line**: Mean (average) time
- **Statistics**: Average time, current percentile, and sample size (toggleable)

## Requirements

- LiveSplit 1.7 or later
- .NET Framework 4.6.1 or later
- Split history data (at least 2 completed attempts for meaningful statistics)

## Building from Source

1. Clone this repository
2. Ensure you have the LiveSplit dependencies in `deps/LiveSplit/`
3. Open `src/LiveSplit.GradingSplits/LiveSplit.GradingSplits.csproj` in Visual Studio
4. Build the solution (Release configuration recommended)
5. The DLL will be output to `deps/LiveSplit/bin/Release/Components/`

## Technical Details

### Statistical Methods
- **Z-Score Calculation**: `(current_time - mean) / standard_deviation`
- **Percentile Conversion**: Uses the cumulative distribution function (CDF) for normal distribution
  - Implemented via the error function (Abramowitz & Stegun approximation)
- **Inverse Conversion**: Uses Beasley-Springer-Moro algorithm for percentile → z-score

### Data Requirements
- Minimum 2 attempts needed for standard deviation calculation
- More attempts = more accurate statistical representation
- Graph visualization benefits from 10+ attempts for clear patterns

### Backward Compatibility
Old settings files using z-score thresholds are automatically converted to percentiles on load.

## Contributing

Contributions are welcome! Feel free to:
- Report bugs via GitHub Issues
- Suggest features or improvements
- Submit pull requests

## License

This component is provided as-is for use with LiveSplit. See the LiveSplit license for more details.

## Credits

- Built for [LiveSplit](https://livesplit.org/)
- Statistical algorithms based on standard normal distribution theory
- Error function approximation from Abramowitz & Stegun
- Inverse normal CDF using Beasley-Springer-Moro algorithm

## Screenshots

*(Add screenshots here showing the component in action, settings panel, and graph visualization)*

## FAQ

**Q: Why percentiles instead of raw times?**  
A: Percentiles normalize performance across splits of different lengths and variability, making grades more meaningful and comparable.

**Q: What if I don't have enough history data?**  
A: The component needs at least 2 attempts to calculate statistics. With fewer attempts, the grades may not be as meaningful. Aim for 10+ attempts for reliable grading.

**Q: Can I use this for individual levels/categories?**  
A: Yes! The component analyzes the currently loaded splits file, so switch to different splits files for different categories.

**Q: The graph looks strange/empty**  
A: This usually means you have very few attempts or very consistent times (low standard deviation). The graph works best with varied data.

**Q: Can I export my grading settings?**  
A: Settings are stored in your LiveSplit layout file. You can share your layout file to share your grading configuration.

**Q: My custom icons aren't showing up**  
A: Check that your icon files are named exactly like your grade labels (case-sensitive on some systems). For example, if your grade is "S", the file should be `S.png`, not `s.png`. Also ensure the icon folder path is correctly set in settings.

**Q: The component crashes or LiveSplit becomes unstable**  
A: Try deleting and re-extracting LiveSplit, then reinstalling the component. Always keep backups of your splits and layouts before updating the component.
