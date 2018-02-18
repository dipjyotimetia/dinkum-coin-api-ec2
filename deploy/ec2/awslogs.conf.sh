#!/bin/bash
cat <<END
[general]
state_file = /var/lib/awslogs/agent-state

[applog]
log_group_name = ${AppLogGroupName}
log_stream_name = ${VersionToDeploy}
file = /opt/PointsService/logs/*.log

[/var/log/messages]
datetime_format = %b %d %H:%M:%S
file = /var/log/messages
buffer_duration = 5000
log_group_name = ${SysLogGroupName}
log_stream_name = ${VersionToDeploy}
END
