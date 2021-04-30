import React, { useEffect } from 'react';
import { Grid } from 'semantic-ui-react';
import ActivityList from './ActivityList';
import { useStore } from '../../../app/stores/store';
import { observer } from 'mobx-react-lite';
import LoadingComponent from '../../../app/layout/LoadingComponent';
import ActivityFilters from './ActivityFilters';

export default observer(function ActivityDashboard() {
    /*Custom hook that was created on store file*/
    const {activityStore} = useStore();
    const {loadActivities, activityRegistry} = activityStore;

    useEffect(() => {
        /*Only call API if activities are not in memory */
        if (activityRegistry.size <= 1) loadActivities();     
    }, [activityStore, loadActivities, activityRegistry.size])

    /*Loading screen*/
    if(activityStore.loadingInitial) return <LoadingComponent content='Loading activities...'></LoadingComponent>

    return (
        <Grid>
            <Grid.Column width='10'>
                <ActivityList></ActivityList>
            </Grid.Column>
            <Grid.Column width='6'>
                <ActivityFilters/>
            </Grid.Column>
        </Grid>
    );
})