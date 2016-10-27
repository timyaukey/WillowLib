How To Add A Table Column
=========================

I haven't actually tried this, but here is my idea:

Change the definition file for the database (Definitions.xml?)

Run the codegen program in the solution.

Create and run the generated SQL conversion script to add the column.

Create and run the generated SQL stored procedure script.

Generate the partial class for the table, and completely
replace the existing generated source file with it.

Regenerate the table in the typed dataset, by deleting and recreating it.

Rebuild the solution to get the new entity class definition.

Fix the entity Validate() method.

Fix any hand coded stored procedures that need it.

If there are any join wrapper classes (e.g. JoinVpToProd), add the new
column to those classes. Do this by adding a snippet of code like the
following somewhere it will be executed, and then pasting the output
from the clipboard into the join wrapper class.

    Willowsoft.WillowLib.CodeGenUI.PersistableWrapperGenerator gen = 
        new Willowsoft.WillowLib.CodeGenUI.PersistableWrapperGenerator("JoinVpToProd");
    gen.Add(typeof(VendorProduct));
    gen.Add(typeof(Product));
    gen.GenerateToClipboard();

Check all uses of the generated entity and/or join classes and see if any
need to be updated. Usually only editors for the updated table MUST be fixed,
but you may WANT to display the information in other places. Editors probably
mean subclassed GridBindingHelpers, mostly adding columns to them.


Special Notes For Copying JoinVpToProd To PurLine
-------------------------------------------------

Sproc which sets order categories must copy the fields we want to dupe.

Conversion script must copy all the data for existing lines.

Forms and reports which use JoinVpToProd copies of copied fields must be
updated to use the PurLine versions. Nothing will break if you do not,
but they may show the wrong data in the future if VendorProduct or Product
values change. Includes the order details grid, various reports printed
from there, item order history, and maybe more.

Must allow new PurLine to be manually added at bottom of order details grid.
It will initially have VendorProductId=null.

Vendor code is optional on manually added rows.

When inserting a new PurLine row, if there is a vendor code ask the user
if they want to add the row to the vendor product list. If they say yes,
insert the rows and set PurLine.VendorProductId so it becomes a generated row.

