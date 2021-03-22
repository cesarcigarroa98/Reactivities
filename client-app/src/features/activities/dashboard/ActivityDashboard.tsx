import React from 'react';
import { Grid } from 'semantic-ui-react';
import ActivityList from './ActivityList';
import ActivityDetails from '../details/ActivityDetails';
import ActivityForm from '../form/ActivityForm';
import { useStore } from '../../../app/stores/store';
import { observer } from 'mobx-react-lite';

export default observer(function ActivityDashboard() {
    const {activityStore} = useStore();
    const {selectedActivity, editMode} = activityStore;

    return (
        <Grid>
            <Grid.Column width='10'>
                <ActivityList></ActivityList>
            </Grid.Column>
            <Grid.Column width='6'>
                {/*Only if selected activity exists and not on edit mode*/}
                {selectedActivity && !editMode &&
                <ActivityDetails></ActivityDetails>}
                {/*Only if editMode is true*/}
                {editMode &&
                <ActivityForm ></ActivityForm>}
            </Grid.Column>
        </Grid>
    )
})