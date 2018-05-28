
import { TourWithManagerForCreation } from './tourWithManagerForCreation.model';
import { ShowforCreation } from '../shows/shared/showForCreation';

export class TourWithManagerAndShowsForCreation extends TourWithManagerForCreation {
  shows: ShowforCreation[];
}
