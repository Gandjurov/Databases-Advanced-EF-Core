<h1>Mini ORM Core</h1>

<h5>This is a sample app, which utilizes the framework. The app is designed to resemble the functionality of <u>Entity Framework Core</u> to an extent. You will be provided with a partially-implemented framework C# project.<h5>

<h3>Project Specification</h3>
<p align="center"><img src="Logo.jpg" alt="CarRentalSystem" width="340"></p>

<h5>The framework should support the following functionality:</h5>
<ul>
	<li>Connecting to a database via provided connection string</li>
	<li><strong>Discovering</strong> entity classes at <strong>runtime</strong></li>
	<li><strong>Retrieving entities</strong> via <strong>framework-generated SQL</strong></li>
	<li><strong>CRUD</strong> operations (inserting, modifying, deleting entities) via <strong>framework-generated SQL</strong></li>
</ul>

<h3>Framework Overview</h3>

<h5>The framework consists of the following <strong>classes</strong>:</h5>
<ul>
	<li><strong>DbSet<T> – Custom generic collection</strong>, which holds the actual <strong>entities</strong> inside it. The <strong>DbContext</strong> class has several <strong>DbSets</strong>, which correspond to all the tables in the database.</li>
	<li><strong>DbContext – Database context</strong> class, responsible for <strong>retrieving entities from the database</strong> and <strong>mapping the relations</strong> between them (through so-called navigation properties).</li>
	<li><strong>DatabaseConnection</strong> – Responsible for <strong>establishing database connections</strong> and <strong>sending SQL queries</strong>. Usually used by the <strong>DbContext</strong>.</li>
	<li><strong>ConnectionManager</strong> – Simple <strong>DatabaseConnection</strong> wrapper, which allows it to be wrapped in a <strong>using</strong> block for <strong>opening and closing connections</strong> to the database</li>
	<li><strong>ChangeTracker</strong> – Responsible for tracking the <strong>added</strong>, <strong>modified</strong> and <strong>deleted</strong> entities from the <strong>DbSets</strong>. Every <strong>DbSet</strong> has one. Used by the <strong>DbContext</strong> to <strong>persist changes</strong> into the database.</li>
	<li><strong>ReflectionHelper<sstrong> – Utility class, which contains some reflection-related methods.</li>
</ul>
