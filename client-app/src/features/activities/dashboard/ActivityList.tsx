import { observer } from 'mobx-react-lite';
import React, { Fragment } from 'react';
import { Header} from 'semantic-ui-react';
import { useStore } from '../../../app/stores/store';
import ActivityListItem from './ActivityListItem';

export default observer(function ActivityList (){
    const {activityStore} = useStore();
    const {groupActivities} = activityStore;

    return (
        <>
            {groupActivities.map(([group, activities]) => (
                /*Whenever a component is called inside a loop, it needs an unique key */
                <Fragment key={group}>
                    <Header sub color='teal'>
                        {group}
                    </Header>
                    {activities.map(activity => (
                        /*Whenever a component is called inside a loop, it needs an unique key */
                        <ActivityListItem key={activity.id} activity={activity}/>
                    ))}
                </Fragment>
            ))}
        </>
    )
})