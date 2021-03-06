<?php require_once '../internals/backend.php'; ?>
<?php require_once '../internals/utils.php'; ?>
<?php init("admin"); ?>
<!doctype html>

<html lang="en">
<head>
	<meta charset="utf-8">
    <link rel="stylesheet" href="pure-min.css"/>
	<link rel="stylesheet" type="text/css" href="admin.css">
</head>

<body id="rootbox">

    <script src="jquery-3.1.0.min.js"></script>

    <h1><a href="index.php">Cannon Conquest | Admin Page</a></h1>

    <?php

    $previd = 0;
	function expansioncell($txt) {
	    global $previd;

	    echo "<td>";
		echo "<a href='#' onclick='ShowExpandedColumn(" . $previd . ", " . str_replace("'", "\\u0027", str_replace('\n', '<br/>', json_encode($txt))) . ");return false;'>show</a>";
		echo "</td>";
    }
    ?>

    <div>
        <h2><a href="ack.php?all=true">Acknowledge all</a></h2>
    <div>

    <div class="tablebox">
        <table class="sqltab pure-table pure-table-bordered">
            <thead>
                <tr>
                    <th>error id</th>
                    <th style='width: 170px'>username</th>
                    <th>anon</th>
                    <th>resolution</th>
                    <th>version</th>
                    <th style="width: 170px;">exception id</th>
                    <th>msg</th>
                    <th>trace</th>
                    <th style='width: 160px'>timestamp</th>
                    <th>additional info</th>
                    <th style='width: 160px'>acknowledged</th>
                </tr>
            </thead>
            <?php foreach (getAllErrors() as $entry): ?>
				<?php 
					if ($entry['acknowledged'] == 1) echo "<tr>"; else echo "<tr style=\"background-color: lightsalmon\">";
				?>
                    <td><?php echo $entry['error_id']; ?></td>
                    <td><a href="userinfo.php?id=<?php echo $entry['userid']; ?>"><?php echo $entry['username']; ?></a> (<?php echo $entry['userid']; ?>)</td>
                    <td><?php echo $entry['password_verified']?0:1; ?></td>
					<?php expansioncell($entry['screen_resolution']); ?>
                    <td><?php echo $entry['app_version']; ?></td>
                    <td><?php echo $entry['exception_id']; ?></td>
					<?php expansioncell($entry['exception_message']); ?>
					<?php expansioncell($entry['exception_stacktrace']); ?>
                    <td><?php echo $entry['timestamp']; ?></td>
					<?php expansioncell($entry['additional_info']); ?>
					<?php 
						if ($entry['acknowledged'] == 0)
						{
							echo "<td>";
							echo $entry['acknowledged'] . " ";
							echo " <a href=\"ack.php?id=" . $entry['error_id'] . "\">(ack)</a>";
							echo " <a href=\"ack.php?exid=" . urlencode($entry['exception_id']) . "\">(ack similiar)</a>";
							echo "</td>";
						} 
						else 
						{
							echo "<td>" . $entry['acknowledged'] . "</td>";
						}
					?>
                </tr>
                <tr class='tab_prev' id='tr_prev_<?php echo $previd; ?>'><td colspan='12' id='td_prev_<?php echo $previd; ?>' style='text-align: left;' ></td></tr>
                <?php $previd++; ?>
            <?php endforeach; ?>
        </table>

    </div>

    <script type="text/javascript">
		<?php echo file_get_contents('admin.js'); ?>
    </script>


</body>
</html>